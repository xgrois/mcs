using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder applicationBuilder, bool isProduction)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProduction);
            }
        }

        private static void SeedData(AppDbContext appDbContext, bool isProduction)
        {
            if (isProduction)
            {
                System.Console.WriteLine("--> Atempting to apply migrations...");
                try
                {
                    appDbContext.Database.Migrate();
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"--> Could not apply migrations: {ex.Message}");
                }
            }


            if (!appDbContext.Platforms.Any())
            {
                Console.WriteLine("--> Seeding data...");
                appDbContext.Platforms.AddRange(
                    new Platform() { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
                    new Platform() { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
                    new Platform() { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
                );
                appDbContext.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> We already have data");
            }
        }

    }
}