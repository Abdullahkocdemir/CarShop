// BusinessLayer.RabbitMQ/EnhancedRabbitMQService.cs
using DTOsLayer.WebApiDTO.RabbitMQ;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization; // Yeni ekleyin

namespace BusinessLayer.RabbitMQ
{
    public class EnhancedRabbitMQService : IDisposable
    {
        private readonly ConnectionFactory _factory;
        private IConnection? _connection;
        private IModel? _channel;

        public EnhancedRabbitMQService()
        {
            _factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "admin",
                Password = "Abdullah159"
            };
        }

        // --- EnsureConnectionAndChannel metodunu buraya taşıdık ---
        private void EnsureConnectionAndChannel()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                _connection = _factory.CreateConnection();
            }

            if (_channel == null || !_channel.IsOpen)
            {
                _channel = _connection.CreateModel();
            }
        }
        // --------------------------------------------------------

        public void PublishEntityMessage<T>(T entity, string operation, string entityType)
        {
            try
            {
                EnsureConnectionAndChannel();
                if (_channel == null)
                {
                    Console.WriteLine(" RabbitMQ kanalı kullanıma hazır değil.");
                    return;
                }

                var exchangeName = $"{entityType.ToLower()}_exchange";
                var routingKey = $"{entityType.ToLower()}.{operation.ToLower()}";

                // Exchange'i declare et
                _channel.ExchangeDeclare(
                    exchange: exchangeName,
                    type: ExchangeType.Topic,
                    durable: true
                );

                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    // PropertyNamingPolicy = JsonNamingPolicy.CamelCase // İsteğe bağlı
                };

                object message;
                message = operation.ToLower() switch
                {
                    "created" => new EntityCreatedMessage<T>
                    {
                        Entity = entity,
                        EntityType = entityType
                    },
                    "updated" => new EntityUpdatedMessage<T>
                    {
                        Entity = entity,
                        EntityType = entityType
                    },
                    "deleted" => new EntityDeletedMessage
                    {
                        EntityId = GetEntityId(entity),
                        EntityType = entityType
                    },
                    _ => throw new ArgumentException($"Unknown operation: {operation}")
                };

                var json = JsonSerializer.Serialize(message, options);
                var body = Encoding.UTF8.GetBytes(json);

                _channel.BasicPublish(
                    exchange: exchangeName,
                    routingKey: routingKey,
                    basicProperties: null,
                    body: body
                );

                Console.WriteLine($" {entityType} {operation} mesajı gönderildi: {routingKey}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" RabbitMQ mesaj gönderme hatası: {ex.Message}");
                throw;
            }
        }

        public void SetupEntityQueues(string entityType)
        {
            try
            {
                EnsureConnectionAndChannel();

                if (_channel == null)
                {
                    Console.WriteLine(" RabbitMQ kanalı kullanıma hazır değil. Kuyruklar oluşturulamıyor.");
                    return;
                }

                var exchangeName = $"{entityType.ToLower()}_exchange";

                _channel.ExchangeDeclare(
                    exchange: exchangeName,
                    type: ExchangeType.Topic,
                    durable: true
                );

                var operations = new[] { "created", "updated", "deleted" };

                foreach (var operation in operations)
                {
                    var queueName = $"{entityType.ToLower()}_{operation}_queue";
                    var routingKey = $"{entityType.ToLower()}.{operation}";

                    _channel.QueueDeclare(
                        queue: queueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );

                    _channel.QueueBind(
                        queue: queueName,
                        exchange: exchangeName,
                        routingKey: routingKey
                    );
                }

                Console.WriteLine($" {entityType} için kuyruklar oluşturuldu.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" {entityType} kuyruk oluşturma hatası: {ex.Message}");
                throw;
            }
        }

        public void SetupAllEntityQueues()
        {
            var entityTypes = new[]
            {
                "Brand", "Product", "Service", "Contact", "Banner",
                "Broadcast", "Feature", "NewLatest", "Showroom", "WhyUse","AboutFeature"
            };

            foreach (var entityType in entityTypes)
            {
                SetupEntityQueues(entityType);
            }
        }

        private int GetEntityId<T>(T entity)
        {
            var property = entity?.GetType().GetProperty($"{entity?.GetType().Name}Id");

            if (property != null)
            {
                object? value = property.GetValue(entity);
                if (value is int id)
                {
                    return id;
                }
            }
            return 0;
        }

        public void Dispose()
        {
            try
            {
                _channel?.Close();
                _connection?.Close();
                Console.WriteLine(" RabbitMQ bağlantısı kapatıldı.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" RabbitMQ kapatma hatası: {ex.Message}");
            }
        }
    }
}