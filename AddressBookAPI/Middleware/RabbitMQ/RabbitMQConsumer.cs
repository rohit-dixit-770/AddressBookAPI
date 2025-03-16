using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Middleware.Email;
using ModelLayer.Model;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Middleware.RabbitMQ
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly IConfiguration _config;
        private readonly IServiceScopeFactory _scopeFactory;

        public RabbitMQConsumer(IConfiguration config, IServiceScopeFactory scopeFactory)
        {
            _config = config;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _config["RabbitMQ:HostName"],
                UserName = _config["RabbitMQ:UserName"],
                Password = _config["RabbitMQ:Password"]
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare("emailQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare("contactQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var eventType = ea.RoutingKey;

                using (var scope = _scopeFactory.CreateScope())
                {
                    if (eventType == "emailQueue")
                    {
                        var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

                        try
                        {
                            if (message.StartsWith("\"") && message.EndsWith("\""))
                            {
                                message = JsonConvert.DeserializeObject<string>(message);
                            }

                            var emailMessage = JsonConvert.DeserializeObject<EmailMessage>(message);

                            if (emailMessage != null)
                            {
                                await emailService.SendEmailAsync(emailMessage.To, emailMessage.Subject, emailMessage.Body);
                                Console.WriteLine($"[Consumer] Processed Email: {emailMessage.To}");
                            }
                            else
                            {
                                Console.WriteLine("[Consumer] EmailMessage is null after deserialization.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[Consumer] Error processing email: {ex.Message}");
                        }
                    }
                    else if (eventType == "contactQueue")
                    {
                        Console.WriteLine($"[Consumer] Contact event received: {message}");
                    }
                }

                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: "emailQueue", autoAck: false, consumer: consumer);
            channel.BasicConsume(queue: "contactQueue", autoAck: false, consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
