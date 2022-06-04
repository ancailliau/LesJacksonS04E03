using AutoMapper;
using Grpc.Core;
using PlatformService.Data;

namespace PlatformService.SyncDataService.gRPC;

public class gRPCPlatformService : GrpcPlatform.GrpcPlatformBase
{
    private readonly IPlatformRepository _platformRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<gRPCPlatformService> _logger;

    public gRPCPlatformService(IPlatformRepository platformRepository, 
        IMapper mapper,
        ILogger<gRPCPlatformService> logger)
    {
        _platformRepository = platformRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public override Task<PlatformResponse> GetAllPlatforms(GetAllRequests request, ServerCallContext context)
    {
        var response = new PlatformResponse();
        var platforms = _platformRepository.GetAll();
        foreach (var platform in platforms)
        {
            response.Platform.Add(_mapper.Map<GrpcPlatformModel>(platform));
        }
        return Task.FromResult(response);
    }
}