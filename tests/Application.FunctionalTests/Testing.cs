using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StonkNotes.Application.FunctionalTests.WebIntegration;
using StonkNotes.Domain.Constants;
using StonkNotes.Infrastructure.Data;
using StonkNotes.Infrastructure.Identity;

namespace StonkNotes.Application.FunctionalTests;

[SetUpFixture]
public partial class Testing
{
    private static ITestDatabase _database = null!;
    private static CustomWebApplicationFactory _factory = null!;
    private static IServiceScopeFactory _scopeFactory = null!;
    private static string? _userId;

    [OneTimeSetUp]
    public async Task RunBeforeAnyTests()
    {
        // provision and configure infrastructure dependencies
        _database = await TestDatabaseFactory.CreateAsync();

        _factory = new CustomWebApplicationFactory(_database.GetConnection());

        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        using var scope = _scopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        return await mediator.Send(request);
    }

    public static async Task SendAsync(IBaseRequest request)
    {
        using var scope = _scopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        await mediator.Send(request);
    }

    public static string? GetUserId()
    {
        return _userId;
    }

    public static async Task<string> RunAsDefaultUserAsync()
    {
        return await RunAsUserAsync("test@local", "Testing1234!", Array.Empty<string>());
    }

    public static async Task<string> RunAsAdministratorAsync()
    {
        return await RunAsUserAsync("administrator@local", "Administrator1234!", new[] { Roles.Administrator });
    }

    public static async Task<string> RunAsUserAsync(string userName, string password, string[] roles)
    {
        // somewhere in here the user is being provisioned in thw wrong db
        using var scope = _scopeFactory.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = new ApplicationUser { UserName = userName, Email = userName };

        var result = await userManager.CreateAsync(user, password);

        if (roles.Any())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }

            await userManager.AddToRolesAsync(user, roles);
        }

        if (result.Succeeded)
        {
            _userId = user.Id;

            return _userId;
        }

        var errors = string.Join(Environment.NewLine, result.ToApplicationResult().Errors);

        throw new Exception($"Unable to create {userName}.{Environment.NewLine}{errors}");
    }

    public static async Task ResetState()
    {
        try
        {
            await _database.ResetAsync();
        }
        catch (Exception) { }

        _userId = null;
    }

    public static async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues)
        where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.FindAsync<TEntity>(keyValues);
    }

    public static async Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Add(entity);

        await context.SaveChangesAsync();
    }

    public static async Task<int> CountAsync<TEntity>() where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.Set<TEntity>().CountAsync();
    }

    public static GraphQLClient GetAnonymousGraphQLClient()
    {
        var httpClient = _factory.CreateClient();
        return new GraphQLClient(httpClient);
    }

    public static async Task<GraphQLClient> GetAuthenticatedGraphQLClient(string username, string password)
    {
        var httpClient = _factory.CreateClient();
        var token = await GetBearerTokenAsync(httpClient, username, password);
        // Add the bearer token to the request headers
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        return new GraphQLClient(httpClient);
    }

    private static async Task<string> GetBearerTokenAsync(HttpClient httpClient, string username, string password)
    {
        // Create the login credentials object that matches your API's expected format
        var loginRequest = new
        {
            Email = username,
            Password = password
        };

        // Convert the credentials to JSON
        var content = new StringContent(
            JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8,
            "application/json");

        // Make the authentication request to your token endpoint
        var response = await httpClient.PostAsync("/api/Users/Login", content);

        // Ensure the request was successful
        response.EnsureSuccessStatusCode();

        // Deserialize the response to get the token
        var tokenResponse = await JsonSerializer.DeserializeAsync<TokenResponse>(
            await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        if (tokenResponse is null)
        {
            throw new InvalidOperationException("Could not deserialize login token. Verify authentication integration.");
        }

        return tokenResponse.AccessToken;
    }

    // Helper class to deserialize the token response
    public class TokenResponse(string accessToken)
    {
        public string AccessToken { get; init; } = accessToken;

        // other properties omitted
    }

    [OneTimeTearDown]
    public async Task RunAfterAnyTests()
    {
        await _database.DisposeAsync();
        await _factory.DisposeAsync();
    }
}
