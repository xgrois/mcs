using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using CommandsService.Models;

namespace CommandsService.Data
{
    public interface ICommandRepository
    {
        bool SaveChanges();

        // Platforms
        IEnumerable<Platform> GetAllPlatforms();
        void CreatePlatform(Platform platform);
        bool PlatformExist(int platfromId);
        bool ExternalPlatformExist(int externalPlatformId);

        // Commands
        IEnumerable<Command> GetCommandsForPlatformId(int platfromId);
        Command GetCommand(int platfromId, int commandId);
        void CreateCommand(int platfromId, Command command);
    }
}