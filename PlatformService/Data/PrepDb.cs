using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;

namespace PlatformService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder builder, bool isProduction)
    {
        using (var serviceScope = builder.ApplicationServices.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetService<AppDbContext>()
                          ?? throw new ArgumentNullException("serviceScope.ServiceProvider.GetService<AppDbContext>()");

            if (isProduction)
            {
                Console.WriteLine("Applying migrations...");
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Could not run migrations... {e.Message}");
                }
            }
                
            SeedData(context);
        }
    }
    private static void SeedData(AppDbContext context)
    {
        if (!context.Platforms.Any())
        {
            context.Platforms.AddRange(
                new() {Name = "Dot Net", Publisher = "Microsoft", Cost = "Free"},
                new() {Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free"},
                new() {Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free"});

            context.SaveChanges();
        }
    }
}