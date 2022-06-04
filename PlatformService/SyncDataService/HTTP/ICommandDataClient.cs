using System.Threading.Tasks;
using PlatformService.DTOs;

namespace PlatformService.SyncDataService.HTTP;

public interface ICommandDataClient
{
    Task SendPlatformToCommand(PlatformReadDto platformReadDto);
}