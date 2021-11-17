using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using PlatformService.DTOs;
using Microsoft.Extensions.Configuration;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }


        public async Task SendPlatformToCommand(PlatformReadDTO platformReadDTO)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(platformReadDTO),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"{_configuration["CommandService"]}", httpContent);

            if (response.IsSuccessStatusCode)
                System.Console.WriteLine("--> Sync POST to CommandService was OK!");
            else
                System.Console.WriteLine("--> SyncDataServices POST to CommandService was NOT OK!");
        }
    }
}