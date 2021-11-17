using PlatformService.DTOs;

namespace PlatformService.AsyncDataServices
{
    public interface IMessageBusClient
    {
        void PublishNewPlatform(PlatformPublishDTO platformPublishDTO);
    }
}