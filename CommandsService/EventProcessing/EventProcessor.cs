using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.DTOs;
using CommandsService.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CommandsService.EventProcessing
{
    enum EventType
    {
        PlatformPublished,
        Undertermined
    }

    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
        {
            _mapper = mapper;
            _serviceScopeFactory = serviceScopeFactory;

        }

        private EventType DetermineEvent(string notificationMessage)
        {
            System.Console.WriteLine("--> Determining event...");

            var eventType = JsonSerializer.Deserialize<GenericEventDTO>(notificationMessage);

            switch (eventType.Event)
            {
                case "Platform_Publish":
                    System.Console.WriteLine("--> Platform Publish Event Detected!");
                    return EventType.PlatformPublished;
                default:
                    System.Console.WriteLine("--> [WARN] Undetermined Event Detected!");
                    return EventType.Undertermined;
            }

        }

        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
                default:
                    break;
            }

        }

        private void AddPlatform(string platformPublishedMessage)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var commandRepository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();

                var platformPublishedDTO = JsonSerializer.Deserialize<PlatformPublishedDTO>(platformPublishedMessage);

                try
                {
                    var platform = _mapper.Map<Platform>(platformPublishedDTO);
                    if (!commandRepository.ExternalPlatformExist(platform.ExternalId))
                    {
                        commandRepository.CreatePlatform(platform);
                        commandRepository.SaveChanges();
                        System.Console.WriteLine("--> Platform Added!");
                    }
                    else
                    {
                        System.Console.WriteLine("--> Platform already exists...!");
                    }
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"--> Could not Add Platform to DB {ex.Message}");
                }
            }
        }


    }

}