using System.Collections.Generic;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CommandsService.Controllers;

[Route("api/c/platforms/{platformId}/[controller]")]
[ApiController]
public class CommandsController : Controller
{
    private readonly ICommandRepository _commandRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PlatformsController> _logger;

    public CommandsController(ICommandRepository commandCommandRepository,
        IMapper mapper,
        ILogger<PlatformsController> logger)
    {
        _commandRepository = commandCommandRepository;
        _mapper = mapper;
        _logger = logger;
    }
        
    [HttpGet]
    public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
    {
        _logger.LogInformation($"Hit GetCommandsForPlatform: {platformId}");

        if (!_commandRepository.PlatformExists(platformId))
        {
            return NotFound();
        }

        var commands = _commandRepository.GetCommandsForPlatform(platformId);
        return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
    }

    [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
    public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
    {
        _logger.LogInformation($"Hit GetCommandForPlatform: {platformId} / {commandId}");

        if (!_commandRepository.PlatformExists(platformId))
        {
            return NotFound();
        }

        var command = _commandRepository.GetCommand(platformId, commandId);
        if(command == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<CommandReadDto>(command));
    }

    [HttpPost]
    public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
    {
        _logger.LogInformation($"Hit CreateCommandForPlatform: {platformId}");

        if (!_commandRepository.PlatformExists(platformId))
        {
            return NotFound();
        }

        var command = _mapper.Map<Command>(commandDto);
        _commandRepository.CreateCommand(platformId, command);
        _commandRepository.SaveChanges();

        var commandReadDto = _mapper.Map<CommandReadDto>(command);

        return CreatedAtRoute(nameof(GetCommandForPlatform),
            new {platformId = platformId, commandId = commandReadDto.Id}, commandReadDto);
    }
}