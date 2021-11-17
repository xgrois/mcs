using System.Data;
using System;
using System.Collections.Generic;
using AutoMapper;
using CommandsService.Data;
using CommandsService.DTOs;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{

    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepository _commandRepository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepository commandRepository, IMapper mapper)
        {
            _mapper = mapper;
            _commandRepository = commandRepository;
        }


        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDTO>> GetCommandsForPlatformId(int platformId)
        {
            System.Console.WriteLine($"--> Hit GetCommandsForPlatformId {platformId}");

            if (!_commandRepository.PlatformExist(platformId))
                return NotFound();

            var commands = _commandRepository.GetCommandsForPlatformId(platformId);

            return Ok(_mapper.Map<IEnumerable<CommandReadDTO>>(commands));
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatformId")]
        public ActionResult<CommandReadDTO> GetCommandForPlatformId(int platformId, int commandId)
        {
            System.Console.WriteLine($"--> Hit GetCommandForPlatformId {platformId} / {commandId}");

            if (!_commandRepository.PlatformExist(platformId))
                return NotFound();

            var command = _commandRepository.GetCommand(platformId, commandId);

            if (command is null)
                return NotFound();

            return Ok(_mapper.Map<CommandReadDTO>(command));
        }


        [HttpPost]
        public ActionResult<CommandReadDTO> CreateCommandForPlatformId(int platformId, CommandCreateDTO commandCreateDTO)
        {
            System.Console.WriteLine($"--> Hit CreateCommandForPlatformId {platformId}");

            if (!_commandRepository.PlatformExist(platformId))
                return NotFound();

            var command = _mapper.Map<Command>(commandCreateDTO);

            _commandRepository.CreateCommand(platformId, command);
            _commandRepository.SaveChanges();

            var commandReadDTO = _mapper.Map<CommandReadDTO>(command);

            return CreatedAtRoute(
                nameof(GetCommandForPlatformId),
                new { platformId = platformId, commandId = commandReadDTO.Id },
                commandReadDTO);
        }

    }
}