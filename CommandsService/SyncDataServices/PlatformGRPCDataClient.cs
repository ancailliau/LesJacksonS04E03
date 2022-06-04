using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandsService.SyncDataServices;

public class PlatformGRPCDataClient : IPlatformDataClient
{
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly ILogger<PlatformGRPCDataClient> _logger;

    public PlatformGRPCDataClient(IConfiguration configuration, IMapper mapper, ILogger<PlatformGRPCDataClient> logger)
    {
        _configuration = configuration;
        _mapper = mapper;
        _logger = logger;
    }
    
    public IEnumerable<Platform> GetAllPlatforms()
    {
        _logger.LogDebug($"Calling GRPC Service {_configuration["GrpcPlatform"]}");

        var channel = GrpcChannel.ForAddress(_configuration["GrpcPlatform"]);
        var client = new GrpcPlatform.GrpcPlatformClient(channel);
        var request = new GetAllRequest();

        try
        {
            var reply = client.GetAllPlatforms(request);
            return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Could not call GRPC Server {ex.Message}");
            return null;
        }
    }
}