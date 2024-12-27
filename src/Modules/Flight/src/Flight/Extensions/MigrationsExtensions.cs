using System;
using BuildingBlocks.EFCore;
using Flight.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Flight.Extensions;

public static class MigrationsExtensions
{
    public static IApplicationBuilder UseMigrations(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (!env.IsEnvironment("test"))
        {
            MigrateDatabase(app.ApplicationServices);
            SeedData(app.ApplicationServices);
        }

        return app;
    }

    private static void MigrateDatabase(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<FlightDbContext>();
        context.Database.Migrate();
    }

    private static void SeedData(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var seeders = scope.ServiceProvider.GetServices<IDataSeeder>();
        foreach (var seeder in seeders)
        {
            seeder.SeedAllAsync().GetAwaiter().GetResult();
        }
    }
}
