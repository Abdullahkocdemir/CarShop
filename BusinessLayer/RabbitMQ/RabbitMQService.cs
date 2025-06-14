using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;

namespace BusinessLayer.RabbitMQ
{
    public class RabbitMQService
    {
        private readonly ConnectionFactory _factory;

        public RabbitMQService()
        {
            _factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "admin",     
                Password = "Abdullah159"  
            };
        }

        public void PublishMessage<T>(T message, string exchangeName, string routingKey)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(
                exchange: exchangeName,
                type: ExchangeType.Topic,
                durable: true
            );

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(
                exchange: exchangeName,
                routingKey: routingKey,
                basicProperties: null,
                body: body
            );

            Console.WriteLine($"Message published to exchange '{exchangeName}' with routing key '{routingKey}': {json}");
        }

        public void SetupBrandQueues()
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(
                exchange: "brand_exchange",
                type: ExchangeType.Topic,
                durable: true
            );

            var queues = new[]
            {
                ("brand_created_queue", "brand.created"),
                ("brand_updated_queue", "brand.updated"),
                ("brand_deleted_queue", "brand.deleted")
            };

            foreach (var (queueName, routingKey) in queues)
            {
                // Declare queue
                channel.QueueDeclare(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                channel.QueueBind(
                    queue: queueName,
                    exchange: "brand_exchange",
                    routingKey: routingKey
                );
            }

            Console.WriteLine("Brand queues and bindings have been set up successfully.");
        }
    }
}
