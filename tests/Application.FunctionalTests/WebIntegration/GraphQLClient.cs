using System.Text;
using System.Text.Json;

namespace StonkNotes.Application.FunctionalTests.WebIntegration

{
    /// <summary>
    /// A simple wrapper class that facilitates sending GraphQL Json documents to a GraphQL endpoint hosted by the integration test suite.
    /// </summary>
    public class GraphQLClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpoint;
        private readonly JsonSerializerOptions _serializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        /// <summary>
        /// A simple wrapper class that facilitates sending GraphQL Json documents to a GraphQL endpoint hosted by the integration test suite.
        /// </summary>
        public GraphQLClient(HttpClient httpClient, string endpoint = "/graphql")
        {
            _httpClient = httpClient;
            // ensure latest GQL over HTTP spec (non-legacy) - https://chillicream.com/docs/hotchocolate/v14/server/http-transport
            _httpClient.DefaultRequestHeaders.Accept.Add(new("application/graphql-response+json"));
            _endpoint = endpoint;
        }

        public async Task<GraphQLResponse<T>> PostGraphQLRequest<T>(GraphQLRequest request)
        {
            var json = JsonSerializer.Serialize(request, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_endpoint, content);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<GraphQLResponse<T>>(responseString, _serializerOptions) ??
                   throw new InvalidOperationException("Could not deserialize a null response string.");
        }

        public async Task<string> GetSchema()
        {
            var response = await _httpClient.GetAsync(_endpoint + "/schema");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }

    /// <summary>
    /// A helper class that enables sending a raw query/mutation string as a GraphQL document with accompanying variables.
    /// </summary>
    public class GraphQLRequest(
        string graphqlDocumentString,
        Dictionary<string, object>? variables = null,
        string? operationName = null)
    {
        public string Query { get; set; } = graphqlDocumentString;

        public Dictionary<string, object> Variables { get; set; } = variables ?? new Dictionary<string, object>();

        public string? OperationName { get; set; } = operationName;
    }

    public class GraphQLResponse<T>
    {
        public T? Data { get; set; }
        public List<GraphQLError>? Errors { get; set; }

        public bool HasErrors => Errors?.Any() ?? false;
    }

    public class GraphQLError
    {
        public string? Message { get; set; } = null;
    }
}
