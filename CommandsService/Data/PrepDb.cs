using CommandsService.Data;
using CommandsService.Models;
using CommandsService.SyncDataServices;

namespace CommandsService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder builder)
    {
        using (var serviceScope = builder.ApplicationServices.CreateScope())
        {
            var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
            if (grpcClient != null)
            {
                var platforms = grpcClient.GetAllPlatforms();
                var commandRepository = serviceScope.ServiceProvider.GetService<ICommandRepository>();
                if (commandRepository != null) SeedData(commandRepository, platforms);
                else Console.WriteLine("Could not seed data");
            }
        }
    }

    private static void SeedData(ICommandRepository repository, IEnumerable<Platform> platforms)
    {
        Console.WriteLine($"Seeding new platforms... {platforms.Count()}");
        foreach (var plat in platforms)
        {
            if (!repository.ExternalPlatformExists(plat.ExternalId)) repository.CreatePlatform(plat);
            else Console.WriteLine($"Ignoring... {plat.ExternalId}");
            repository.SaveChanges();
        }
    }
}