using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PlatformService.DTOs;

namespace PlatformService.SyncDataService.HTTP;

public class HttpCommandDataClientService : ICommandDataClient 
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<HttpCommandDataClientService> _logger;

    public HttpCommandDataClientService(HttpClient httpClient,
        IConfiguration configuration,
        ILogger<HttpCommandDataClientService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendPlatformToCommand(PlatformReadDto platformReadDto)
    {
        var httpContent = new StringContent(
            JsonSerializer.Serialize(platformReadDto),
            Encoding.UTF8,
            MediaTypeNames.Application.Json
        );

        var response = await _httpClient.PostAsync($"{_configuration["CommandService"]}/api/c/platforms", httpContent);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("POST Async success");
        }
        else
        {
            _logger.LogError("POST Async failed");
        }
    }
}