using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Collections.Concurrent;
using DTOsLayer.WebApiDTO.BrandDTO.Messages;

namespace BusinessLayer.RabbitMQ
{
    public class RabbitMQConsumerService : IDisposable
    {
        private readonly ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channel;

        // Thread-safe collection kullan
        public static readonly ConcurrentBag<object> ConsumedMessages = new ConcurrentBag<object>();

        public RabbitMQConsumerService(string hostName, string userName, string password)
        {
            _factory = new ConnectionFactory()
            {
                HostName = hostName,
                Port = 5672,
                UserName = userName,
                Password = password
            };
        }

        public void StartConsuming()
        {
            try
            {
                _connection = _factory.CreateConnection();
                _channel = _connection.CreateModel();

                // Exchange'i declare et
                _channel.ExchangeDeclare(
                    exchange: "brand_exchange",
                    type: ExchangeType.Topic,
                    durable: true
                );

                // UI için özel kuyruk
                var consumerQueueName = "web_ui_brand_messages_queue";
                _channel.QueueDeclare(
                    queue: consumerQueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                // Tüm brand mesajlarını dinle
                _channel.QueueBind(
                    queue: consumerQueueName,
                    exchange: "brand_exchange",
                    routingKey: "brand.*"
                );

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += OnMessageReceived;

                _channel.BasicConsume(
                    queue: consumerQueueName,
                    autoAck: false,
                    consumer: consumer
                );

                Console.WriteLine($" [*] RabbitMQ Consumer başlatıldı. Kuyruk: '{consumerQueueName}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RabbitMQ Consumer başlatma hatası: {ex.Message}");
                throw;
            }
        }

        private void OnMessageReceived(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            Console.WriteLine($" [x] Mesaj alındı - Routing Key: {ea.RoutingKey}, Mesaj: {message}");

            try
            {
                object deserializedMessage = null;

                switch (ea.RoutingKey)
                {
                    case "brand.created":
                        deserializedMessage = JsonSerializer.Deserialize<BrandCreatedMessage>(message);
                        Console.WriteLine($" [+] Brand Created: {((BrandCreatedMessage)deserializedMessage).BrandName}");
                        break;

                    case "brand.updated":
                        deserializedMessage = JsonSerializer.Deserialize<BrandUpdatedMessage>(message);
                        Console.WriteLine($" [~] Brand Updated: {((BrandUpdatedMessage)deserializedMessage).BrandName}");
                        break;

                    case "brand.deleted":
                        deserializedMessage = JsonSerializer.Deserialize<BrandDeletedMessage>(message);
                        Console.WriteLine($" [-] Brand Deleted: ID {((BrandDeletedMessage)deserializedMessage).BrandId}");
                        break;

                    default:
                        Console.WriteLine($" [?] Bilinmeyen routing key: {ea.RoutingKey}");
                        deserializedMessage = message; // String olarak sakla
                        break;
                }

                if (deserializedMessage != null)
                {
                    ConsumedMessages.Add(deserializedMessage);
                    Console.WriteLine($" [✓] Mesaj eklendi. Toplam mesaj sayısı: {ConsumedMessages.Count}");
                }

                // Mesajı onayla
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($" [!] JSON Parse Hatası: {ex.Message}");
                ConsumedMessages.Add(message); // Hatalı mesajı string olarak sakla
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" [!] Mesaj işleme hatası: {ex.Message}");
                _channel.BasicNack(ea.DeliveryTag, false, true); // Mesajı geri kuyruğa koy
            }
        }

        // Mesaj sayılarını almak için yardımcı method
        public MessageCounts GetMessageCounts()
        {
            var counts = new MessageCounts();

            foreach (var message in ConsumedMessages)
            {
                switch (message)
                {
                    case BrandCreatedMessage:
                        counts.CreatedCount++;
                        break;
                    case BrandUpdatedMessage:
                        counts.UpdatedCount++;
                        break;
                    case BrandDeletedMessage:
                        counts.DeletedCount++;
                        break;
                }
            }

            return counts;
        }

        public void Dispose()
        {
            try
            {
                _channel?.Close();
                _connection?.Close();
                Console.WriteLine(" [x] RabbitMQ bağlantısı kapatıldı.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" [!] RabbitMQ kapatma hatası: {ex.Message}");
            }
        }
    }

    public class MessageCounts
    {
        public int CreatedCount { get; set; }
        public int UpdatedCount { get; set; }
        public int DeletedCount { get; set; }
        public int TotalCount => CreatedCount + UpdatedCount + DeletedCount;
    }
}