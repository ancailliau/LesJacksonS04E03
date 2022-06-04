using System;
using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CommandsService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;
    private readonly ILogger<EventProcessor> _logger;

    public EventProcessor(IServiceScopeFactory scopeFactory,
        AutoMapper.IMapper mapper,
        ILogger<EventProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
        _logger = logger;
    }

    public void ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);

        switch (eventType)
        {
            case EventType.PlatformPublished:
                AddPlatform(message);
                break;
        }
    }

    private EventType DetermineEvent(string notifcationMessage)
    {
        _logger.LogInformation("Determining Event");

        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notifcationMessage);

        if (eventType != null)
            switch (eventType.Event)
            {
                case "Platform_Published":
                    _logger.LogInformation("Platform Published Event Detected");
                    return EventType.PlatformPublished;
            }
            
        _logger.LogInformation("Could not determine the event type");
        return EventType.Undefined;
    }

    private void AddPlatform(string platformPublishedMessage)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var commandRepository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();
            var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

            try
            {
                var platform = _mapper.Map<Platform>(platformPublishedDto);
                if(!commandRepository.ExternalPlatformExists(platform.ExternalId))
                {
                    commandRepository.CreatePlatform(platform);
                    commandRepository.SaveChanges();
                    _logger.LogInformation("Platform added!");
                }
                else
                {
                    _logger.LogInformation("Platform already exists...");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not add Platform to DB {ex.Message}");
            }
        }
    }
}
    
enum EventType {
    PlatformPublished, Undefined
}