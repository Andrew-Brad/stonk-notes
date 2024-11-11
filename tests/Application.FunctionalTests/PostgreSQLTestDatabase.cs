using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Respawn;
using StonkNotes.Application.FunctionalTests;
using StonkNotes.Infrastructure.Data;

namespace CleanArchitecture.Application.FunctionalTests;

public class PostgreSQLTestDatabase : ITestDatabase
{
    private readonly string _connectionString = null!;
    private NpgsqlConnection _connection = null!;
    private Respawner _respawner = null!;

    public PostgreSQLTestDatabase()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        _connectionString = Guard.Against.Null(configuration.GetConnectionString("IntegrationTestDb"));
    }

    public async Task InitialiseAsync()
    {
        _connection = new NpgsqlConnection(_connectionString);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_connectionString)
            //.ConfigureWarnings(warnings => warnings.Log(RelationalEventId.PendingModelChangesWarning))
            .Options;

        var context = new ApplicationDbContext(options);

        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();

        await _connection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            TablesToIgnore = ["__EFMigrationsHistory"],
            //WithReseed = true
        });
        await _connection.CloseAsync();
    }

    public DbConnection GetConnection() => new NpgsqlConnection(_connectionString);

    public string GetConnectionString() => _connectionString;

    public async Task ResetAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
    }

    public async Task DisposeAsync() => await _connection.DisposeAsync();
}
