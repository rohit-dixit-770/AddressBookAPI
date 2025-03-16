using System;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Middleware.RabbitMQ
{
    public class RabbitMQProducer
    {
        private readonly IConfiguration _config;

        public RabbitMQProducer(IConfiguration config)
        {
            _config = config;
        }

        public void PublishMessage(string queueName, object message)
        {
            var hostName = _config["RabbitMQ:HostName"];
            var userName = _config["RabbitMQ:UserName"];
            var password = _config["RabbitMQ:Password"];

            var factory = new ConnectionFactory
            {
                HostName = hostName,
                UserName = userName,
                Password = password
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var jsonMessage = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: properties,
                body: body
            );

            Console.WriteLine($"[Producer] Sent Message to {queueName}: {jsonMessage}");
        }
    }
}
