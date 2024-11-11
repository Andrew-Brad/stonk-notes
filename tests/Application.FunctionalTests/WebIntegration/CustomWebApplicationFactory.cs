using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StonkNotes.Application.Common.Interfaces;
using StonkNotes.Infrastructure.Data;
using StonkNotes.Web;

namespace StonkNotes.Application.FunctionalTests;

using static Testing;

// Todo: Move this type and associated Web Integration tests to their own test project
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly DbConnection _connection;

    public CustomWebApplicationFactory(DbConnection connection)
    {
        _connection = connection;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services
                .RemoveAll<IUser>()
                .AddTransient(provider => Mock.Of<IUser>(s => s.Id == GetUserId()));

            services
                .RemoveAll<DbContextOptions<ApplicationDbContext>>()
                 .AddDbContext<ApplicationDbContext>((sp, options) =>
                 {
                     options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                     options.UseNpgsql(_connection);
                 });
        });
    }
}
