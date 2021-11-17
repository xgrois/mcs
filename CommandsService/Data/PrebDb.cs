using System.Collections.Generic;
using System.Windows.Input;
using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CommandsService.Data
{
    public static class PrebDb
    {
        public static void PrepPopulation(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();

                var platforms = grpcClient.ReturnAllPlatforms();

                SeedData(serviceScope.ServiceProvider.GetService<ICommandRepository>(), platforms);
            }
        }

        private static void SeedData(ICommandRepository commandRepository, IEnumerable<Platform> platforms)
        {
            System.Console.WriteLine("--> Seeding new platforms");

            foreach (var plat in platforms)
            {
                if (!commandRepository.ExternalPlatformExist(plat.ExternalId))
                {
                    commandRepository.CreatePlatform(plat);
                }
                commandRepository.SaveChanges();
            }
        }
    }
}