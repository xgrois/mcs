using System;
using System.Collections.Generic;
using System.Linq;
using CommandsService.Models;

namespace CommandsService.Data
{
    public class CommandRepository : ICommandRepository
    {
        private readonly AppDbContext _appDbContext;

        public CommandRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }


        public void CreateCommand(int platfromId, Command command)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            command.PlatformId = platfromId;
            _appDbContext.Commands.Add(command);
        }

        public void CreatePlatform(Platform platform)
        {
            if (platform is null)
                throw new ArgumentNullException(nameof(platform));

            _appDbContext.Platforms.Add(platform);
        }

        public bool ExternalPlatformExist(int externalPlatformId)
        {
            return _appDbContext.Platforms.Any(p => p.ExternalId == externalPlatformId);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _appDbContext.Platforms.ToList();
        }

        public Command GetCommand(int platfromId, int commandId)
        {
            return _appDbContext.Commands
                        .Where(c => c.PlatformId == platfromId && c.Id == commandId)
                        .FirstOrDefault();
        }

        public IEnumerable<Command> GetCommandsForPlatformId(int platfromId)
        {
            return _appDbContext.Commands
                .Where(c => c.PlatformId == platfromId)
                .OrderBy(c => c.Platform.Name);
        }

        public bool PlatformExist(int platfromId)
        {
            return _appDbContext.Platforms.Any(p => p.Id == platfromId);
        }

        public bool SaveChanges()
        {
            return (_appDbContext.SaveChanges() > 0);
        }
    }
}