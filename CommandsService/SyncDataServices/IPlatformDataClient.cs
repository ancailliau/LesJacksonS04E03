using CommandsService.Models;

namespace CommandsService.SyncDataServices;

public interface IPlatformDataClient
{
    IEnumerable<Platform> GetAllPlatforms();
}