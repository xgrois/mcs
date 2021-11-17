using System.Text;
using System;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PlatformService.DTOs;
using RabbitMQ.Client;


namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        // Comments
        /*sdsd*/
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration configuration)
        {

            _configuration = configuration;
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                System.Console.WriteLine("--> Connected to Message Bus");
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
            }

        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            System.Console.WriteLine("--> RabbitMQ Connection Shutdown");
        }

        public void PublishNewPlatform(PlatformPublishDTO platformPublishDTO)
        {
            var message = JsonSerializer.Serialize(platformPublishDTO);

            if (_connection.IsOpen)
            {
                System.Console.WriteLine("--> RabbitMQ Connection Open, sending message...");
                SendMessage(message);
            }
            else
            {
                System.Console.WriteLine("--> RabbitMQ connection is closed, not sending anywhing...");
            }

        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body: body);
            System.Console.WriteLine($"--> Message sent to RabbitMQ! The content sent was: {message}");
        }

        public void Dispose()
        {
            System.Console.Write("--> Disposing MessageBusClient...");
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
            System.Console.WriteLine("DONE!");
        }

    }
}