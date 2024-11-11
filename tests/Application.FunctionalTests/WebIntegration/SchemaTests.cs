using static StonkNotes.Application.FunctionalTests.Testing;

namespace StonkNotes.Application.FunctionalTests.WebIntegration;

public class SchemaTests
{
    /// <summary>
    /// If this test fails then you need to regenerate the latest schema. See Readme.md in the root directory.
    /// </summary>
    [Test]
    public async Task GraphQLSchemaShouldMatchSnapshot()
    {
        var client = GetAnonymousGraphQLClient();
        var latestSchemaSdlFromWebApp = await client.GetSchema();
        var schemaFilePath = GetSchemaFilePath();
        var expectedSchema = await File.ReadAllTextAsync(schemaFilePath);
        latestSchemaSdlFromWebApp.Should().BeEquivalentTo(expectedSchema,
            because: "schema changes require exporting of the graphql schema. See readme.me in the root directory.");
    }

    private static string GetSchemaFilePath()
    {
        var projectRoot = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent?.Parent;
        var schemaFilePath = Path.Combine(projectRoot?.FullName ?? string.Empty, "src", "Web", "GraphQLSchema",
            "schema.graphql");

        if (!File.Exists(schemaFilePath))
        {
            throw new FileNotFoundException($"The schema file was not found at path: {schemaFilePath}");
        }

        return schemaFilePath;
    }
}
