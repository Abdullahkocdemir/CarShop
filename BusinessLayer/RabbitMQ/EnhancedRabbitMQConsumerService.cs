using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DTOsLayer.WebApiDTO.RabbitMQ;

namespace BusinessLayer.RabbitMQ
{
    public class EnhancedRabbitMQConsumerService : IDisposable
    {
        private readonly ConnectionFactory _factory;
        private IConnection? _connection;
        private IModel? _channel;

        public static readonly ConcurrentBag<BaseMessage> AllMessages = new ConcurrentBag<BaseMessage>();

        public static readonly ConcurrentDictionary<string, ConcurrentBag<BaseMessage>> MessagesByEntity
            = new ConcurrentDictionary<string, ConcurrentBag<BaseMessage>>();

        public EnhancedRabbitMQConsumerService(string hostName, string userName, string password)
        {
            _factory = new ConnectionFactory()
            {
                HostName = hostName,
                Port = 5672,
                UserName = userName,
                Password = password
            };
        }

        public void StartConsumingAllEntities()
        {
            try
            {
                _connection = _factory.CreateConnection();
                _channel = _connection.CreateModel();

                var entityTypes = new[]
                {
                    "brand", "product", "service", "contact", "banner",
                    "broadcast", "feature", "newlatest", "showroom", "whyuse"
                };

                foreach (var entityType in entityTypes)
                {
                    SetupConsumerForEntity(entityType);
                }

                Console.WriteLine(" Tüm entity'ler için RabbitMQ Consumer başlatıldı!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" RabbitMQ Consumer başlatma hatası: {ex.Message}");
                throw;
            }
        }

        private void SetupConsumerForEntity(string entityType)
        {
            if (_channel == null)
            {
                Console.WriteLine(" Kanal başlatılmamış! Consumer kurulamıyor.");
                return;
            }

            var exchangeName = $"{entityType}_exchange";

            // Exchange'i declare et
            _channel.ExchangeDeclare(
                exchange: exchangeName,
                type: ExchangeType.Topic,
                durable: true
            );

            // Web UI için özel kuyruk
            var consumerQueueName = $"web_ui_{entityType}_messages_queue";
            _channel.QueueDeclare(
                queue: consumerQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            // Tüm operasyonları dinle
            _channel.QueueBind(
                queue: consumerQueueName,
                exchange: exchangeName,
                routingKey: $"{entityType}.*"
            );

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) => OnMessageReceived(model, ea, entityType);

            _channel.BasicConsume(
                queue: consumerQueueName,
                autoAck: false,
                consumer: consumer
            );

            Console.WriteLine($" {entityType.ToUpper()} mesajları dinleniyor...");
        }

        private void OnMessageReceived(object? model, BasicDeliverEventArgs ea, string entityType) // model? olarak değiştirildi
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            Console.WriteLine($" {entityType.ToUpper()} mesajı alındı - Routing Key: {ea.RoutingKey}");

            try
            {
                BaseMessage? deserializedMessage = null;
                var operation = ea.RoutingKey.Split('.')[1];

                switch (operation)
                {
                    case "created":
                        deserializedMessage = JsonSerializer.Deserialize<EntityCreatedMessage<object>>(message);
                        Console.WriteLine($" {entityType.ToUpper()} Created");
                        break;

                    case "updated":
                        deserializedMessage = JsonSerializer.Deserialize<EntityUpdatedMessage<object>>(message);
                        Console.WriteLine($"✏️ {entityType.ToUpper()} Updated");
                        break;

                    case "deleted":
                        deserializedMessage = JsonSerializer.Deserialize<EntityDeletedMessage>(message);
                        Console.WriteLine($" {entityType.ToUpper()} Deleted");
                        break;

                    default:
                        Console.WriteLine($" Bilinmeyen operasyon: {operation}");
                        break;
                }

                if (deserializedMessage != null)
                {
                    // Genel listeye ekle
                    AllMessages.Add(deserializedMessage);

                    // Entity tipine özel listeye ekle
                    // TryAdd kullanmak daha güvenli ve ConcurrentDictionary'nin yapısına daha uygun
                    MessagesByEntity.TryAdd(entityType, new ConcurrentBag<BaseMessage>());
                    MessagesByEntity[entityType].Add(deserializedMessage);

                    Console.WriteLine($" Mesaj kaydedildi. Toplam: {AllMessages.Count}");
                }

                // _channel null olabileceği için kontrol ekliyoruz, ancak bu metod sadece kanal aktifken çağrılmalı
                _channel?.BasicAck(ea.DeliveryTag, false);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($" JSON Parse Hatası: {ex.Message}");
                _channel?.BasicAck(ea.DeliveryTag, false); // Hata durumunda da acknowledge etmeli
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Mesaj işleme hatası: {ex.Message}");
                // _channel null olabileceği için kontrol ekliyoruz
                _channel?.BasicNack(ea.DeliveryTag, false, true);
            }
        }

        public Dictionary<string, EntityMessageCounts> GetAllEntityMessageCounts()
        {
            var result = new Dictionary<string, EntityMessageCounts>();

            foreach (var kvp in MessagesByEntity)
            {
                var entityType = kvp.Key;
                var messages = kvp.Value;

                var counts = new EntityMessageCounts
                {
                    EntityType = entityType
                };

                foreach (var message in messages)
                {
                    switch (message.Operation)
                    {
                        case "Created":
                            counts.CreatedCount++;
                            break;
                        case "Updated":
                            counts.UpdatedCount++;
                            break;
                        case "Deleted":
                            counts.DeletedCount++;
                            break;
                    }
                }

                result[entityType] = counts;
            }

            return result;
        }

        public void Dispose()
        {
            try
            {
                _channel?.Close();
                _connection?.Close();
                Console.WriteLine("🔌 RabbitMQ Consumer bağlantısı kapatıldı.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ RabbitMQ Consumer kapatma hatası: {ex.Message}");
            }
        }
    }
    public class EntityMessageCounts
    {
        public string EntityType { get; set; } = string.Empty;
        public int CreatedCount { get; set; }
        public int UpdatedCount { get; set; }
        public int DeletedCount { get; set; }
        public int TotalCount => CreatedCount + UpdatedCount + DeletedCount;
    }


}