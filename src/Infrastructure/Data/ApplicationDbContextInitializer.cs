using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StonkNotes.Domain.Constants;
using StonkNotes.Domain.Entities;
using StonkNotes.Domain.ValueObjects;
using StonkNotes.Infrastructure.Identity;

namespace StonkNotes.Infrastructure.Data;

public static class InitializerExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitializer(
    ILogger<ApplicationDbContextInitializer> logger,
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager)
{
    public async Task InitialiseAsync()
    {
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while Migrating the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database. Did you apply a migration and forget to update seeding code?");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        // Default roles
        var administratorRole = new IdentityRole(Roles.Administrator);

        if (roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await roleManager.CreateAsync(administratorRole);
        }

        // Default users
        var administrator = new ApplicationUser { UserName = "administrator@localhost", Email = "administrator@localhost" };

        if (userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await userManager.CreateAsync(administrator, "Administrator1!");
            if (!string.IsNullOrWhiteSpace(administratorRole.Name))
            {
                await userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
            }
        }

        // Seed default local data as needed
        if (!context.DayNotes.Any())
        {
            context.DayNotes.Add(new(DateOnly.FromDayNumber(1), ShortText.From("Summary 1"), LongText.From("Long Text"),
                MarketCondition.Bearish, Mood.Neutral, MarketVolatility.High));
            context.DayNotes.Add(new(DateOnly.FromDayNumber(2), ShortText.From("Summary 2"), LongText.From("Long Text"),
                MarketCondition.Bearish, Mood.Neutral, MarketVolatility.Moderate));
            context.DayNotes.Add(new(DateOnly.FromDateTime(DateTime.UnixEpoch.Date), ShortText.From("Summary 3"), LongText.From("Long Text"),
                MarketCondition.Bullish, Mood.Sad, MarketVolatility.Low));
            context.DayNotes.Add(new(DateOnly.FromDateTime(DateTime.Today), ShortText.From("Summary 4"), LongText.From(new(Enumerable.Repeat('a', LongText.MaxLength).ToArray())),
                MarketCondition.Neutral, Mood.Happy, MarketVolatility.High));
        }

        await context.SaveChangesAsync();
    }
}
