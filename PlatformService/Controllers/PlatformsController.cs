using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.DTOs;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;
using System.Threading.Tasks;
using PlatformService.AsyncDataServices;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepository _platformRepository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;
        public PlatformsController(IPlatformRepository platformRepository, IMapper mapper, ICommandDataClient commandDataClient, IMessageBusClient messageBusClient)
        {
            _messageBusClient = messageBusClient;
            _commandDataClient = commandDataClient;
            _mapper = mapper;
            _platformRepository = platformRepository;

        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDTO>> GetPlatforms()
        {
            System.Console.WriteLine("--> Getting platforms...");

            var platforms = _platformRepository.GetAllPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDTO>>(platforms));
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDTO> GetPlatformById(int id)
        {
            System.Console.WriteLine($"--> Getting platform {id}...");

            var platform = _platformRepository.GetPlatformById(id);

            if (platform is null)
                return NotFound();

            return Ok(_mapper.Map<PlatformReadDTO>(platform));
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDTO>> CreatePlatform(PlatformCreateDTO platformCreateDTO)
        {
            System.Console.WriteLine($"--> Creating new platform...");

            var platform = _mapper.Map<Platform>(platformCreateDTO);


            _platformRepository.CreatePlatform(platform);
            _platformRepository.SaveChanges();

            var platformReadDTO = _mapper.Map<PlatformReadDTO>(platform);

            // Send sync message
            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDTO);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Could not send synchronously: {ex.Message}");
            }

            // Send async message
            try
            {
                var platformPublishDTO = _mapper.Map<PlatformPublishDTO>(platformReadDTO);
                platformPublishDTO.Event = "Platform_Publish";
                _messageBusClient.PublishNewPlatform(platformPublishDTO);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Could not send asynchronously: {ex.Message}");
            }

            return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDTO.Id }, platformReadDTO);
        }


    }
}