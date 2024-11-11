using System.Reflection;
using HotChocolate.Authorization;
using HotChocolate.Configuration;
using HotChocolate.Execution.Configuration;
using StonkNotes.Web.GraphQLSchema;

namespace StonkNotes.Web.Infrastructure;

public static class WebApplicationExtensions
{
    public static RouteGroupBuilder MapGroup(this WebApplication app, EndpointGroupBase group)
    {
        var groupName = group.GetType().Name;

        return app
            .MapGroup($"/api/{groupName}")
            .WithGroupName(groupName)
            .WithTags(groupName)
            .WithOpenApi();
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var endpointGroupType = typeof(EndpointGroupBase);

        var assembly = Assembly.GetExecutingAssembly();

        var endpointGroupTypes = assembly.GetExportedTypes()
            .Where(t => t.IsSubclassOf(endpointGroupType));

        foreach (var type in endpointGroupTypes)
        {
            if (Activator.CreateInstance(type) is EndpointGroupBase instance)
            {
                instance.Map(app);
            }
        }

        return app;
    }

    public static IRequestExecutorBuilder AddGraphQLRequestExecutorBuilder(
        this WebApplicationBuilder webApplicationBuilder)
    {
        var requestExecutorBuilder = webApplicationBuilder
            .AddGraphQL()
            .AddAuthorization()
            .ConfigureSchema(ConfigureSchema)
            .AddMutationConventions()
            .InitializeOnStartup();
        return requestExecutorBuilder;
    }

    private static void ConfigureSchema(this ISchemaBuilder schemaBuilder)
    {
        schemaBuilder
            .AddQueryType<QueryType>()
            .AddMutationType<MutationType>()
            .AddType<DayNoteType>();


        schemaBuilder.ModifyOptions(options =>
        {
            options.RemoveUnusedTypeSystemDirectives = true;
        });
    }
}
