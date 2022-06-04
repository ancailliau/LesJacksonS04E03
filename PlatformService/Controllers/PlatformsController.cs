using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlatformService.AsyncDataService;
using PlatformService.Data;
using PlatformService.DTOs;
using PlatformService.Models;
using PlatformService.SyncDataService.HTTP;

namespace PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformRepository _platformRepository;
    private readonly ICommandDataClient _commandDataClient;
    private readonly IMessageBusClient _messageBusClient;
    private readonly IMapper _mapper;
    private readonly ILogger<PlatformsController> _logger;

    public PlatformsController(IPlatformRepository platformRepository,
        ICommandDataClient commandDataClient,
        IMessageBusClient messageBusClient,
        IMapper mapper,
        ILogger<PlatformsController> logger)
    {
        _platformRepository = platformRepository;
        _commandDataClient = commandDataClient;
        _messageBusClient = messageBusClient;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        _logger.LogDebug("Get all platforms");
        var items = _platformRepository.GetAll();
        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(items));
    }
        
    [HttpGet("{id:int}", Name="GetPlatformById")]
    public ActionResult<PlatformReadDto> GetPlatformById(int id)
    {
        _logger.LogDebug($"Get platform {{id}}");
        var item = _platformRepository.GetById(id);
        if (item != null)
            return Ok(_mapper.Map<PlatformReadDto>(item));
        return NotFound();
    }
        
    [HttpPost(Name = "CreatePlatform")]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
    {
        var item = _mapper.Map<Platform>(platformCreateDto);
        _platformRepository.Create(item);
        _platformRepository.SaveChanges();
            
        var platformReadDto = _mapper.Map<PlatformReadDto>(item);

        try
        {
            await _commandDataClient.SendPlatformToCommand(platformReadDto);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not reach command service: {e.Message}");
        }

        try
        {
            var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
            platformPublishedDto.Event = "Platform_Published";
            _messageBusClient.PublishNewPlatform(platformPublishedDto);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Could not send asynchronous: {e.Message}");
        }
            
        return CreatedAtRoute(nameof(GetPlatformById), new { id = platformReadDto.Id }, platformReadDto);
    }
}