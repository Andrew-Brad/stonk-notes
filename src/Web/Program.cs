using StonkNotes.Infrastructure.Data;

namespace StonkNotes.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        ConfigureServices(builder);

        var app = builder.Build();

        await ConfigureAsync(app, args);
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddKeyVaultIfConfigured(builder.Configuration);

        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddWebServices();

        builder.AddGraphQLRequestExecutorBuilder();
    }

    private static async Task ConfigureAsync(WebApplication app, string[] args)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            await app.InitialiseDatabaseAsync();
        }
        else
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHealthChecks("/healthz");
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseSwaggerUi(settings =>
        {
            settings.Path = "/api";
            settings.DocumentPath = "/api/specification.json";
        });
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller}/{action=Index}/{id?}");
        app.MapRazorPages();
        app.MapFallbackToFile("index.html");
        app.UseExceptionHandler(options => { });
        app.Map("/", () => Results.Redirect("/graphql"));
        app.MapEndpoints();
        app.MapGraphQL();

        // Start the application
        app.RunWithGraphQLCommands(args);
    }
}
