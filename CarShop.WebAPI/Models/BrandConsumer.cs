using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json; // JsonSerializer için eklendi

public class BrandConsumer
{
    public static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost", UserName = "admin", Password = "Abdullah159" }; // Kendi bağlantı bilgilerinizle değiştirin
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            // Exchange'i declare et (publisher'da da yapıldığı gibi, burada da var olduğundan emin olmak için)
            channel.ExchangeDeclare(exchange: "brand_exchange", type: ExchangeType.Topic, durable: true);

            // ----------------------------------------------------
            // 1. Oluşturulan markalar için bir kuyruk tanımlayın
            // ----------------------------------------------------
            string createdQueueName = "brand_created_events_queue";
            channel.QueueDeclare(queue: createdQueueName,
                                 durable: true,    // Kalıcı kuyruk: RabbitMQ yeniden başlasa bile mesajlar korunur
                                 exclusive: false, // Dışarıdan erişilebilir
                                 autoDelete: false // Son tüketici ayrıldığında silinmez
                                );

            // Bu kuyruğu "brand_exchange"e "brand.created" anahtarıyla bağlayın
            channel.QueueBind(queue: createdQueueName,
                              exchange: "brand_exchange",
                              routingKey: "brand.created");
            Console.WriteLine($"Kuyruk '{createdQueueName}' oluşturuldu ve 'brand.created' ile bağlandı.");

            // ----------------------------------------------------
            // 2. Güncellenen markalar için bir kuyruk tanımlayın
            // ----------------------------------------------------
            string updatedQueueName = "brand_updated_events_queue";
            channel.QueueDeclare(queue: updatedQueueName, durable: true, exclusive: false, autoDelete: false);
            channel.QueueBind(queue: updatedQueueName, exchange: "brand_exchange", routingKey: "brand.updated");
            Console.WriteLine($"Kuyruk '{updatedQueueName}' oluşturuldu ve 'brand.updated' ile bağlandı.");

            // ----------------------------------------------------
            // 3. Silinen markalar için bir kuyruk tanımlayın
            // ----------------------------------------------------
            string deletedQueueName = "brand_deleted_events_queue";
            channel.QueueDeclare(queue: deletedQueueName, durable: true, exclusive: false, autoDelete: false);
            channel.QueueBind(queue: deletedQueueName, exchange: "brand_exchange", routingKey: "brand.deleted");
            Console.WriteLine($"Kuyruk '{deletedQueueName}' oluşturuldu ve 'brand.deleted' ile bağlandı.");

            // ----------------------------------------------------
            // İsteğe bağlı: Tüm brand mesajları için tek bir kuyruk
            // ----------------------------------------------------
            string allBrandEventsQueue = "brand_all_events_queue";
            channel.QueueDeclare(queue: allBrandEventsQueue, durable: true, exclusive: false, autoDelete: false);
            // '#' wildcard'ı "brand." ile başlayan tüm routing key'leri yakalar
            channel.QueueBind(queue: allBrandEventsQueue, exchange: "brand_exchange", routingKey: "brand.#");
            Console.WriteLine($"Kuyruk '{allBrandEventsQueue}' oluşturuldu ve 'brand.#' ile bağlandı.");


            Console.WriteLine(" [*] Mesajlar bekleniyor. Çıkmak için CTRL+C tuşuna basın.");

            // Tüm kuyruklar için ortak bir tüketici olayı oluştur
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;
                var consumerTag = ea.ConsumerTag; // Hangi consumer'ın aldığını görmek için

                Console.WriteLine($" [x] Kuyruk '{consumerTag}' -> Routing Key: '{routingKey}' -> Mesaj: '{message}'");

                // Mesajı işledikten sonra RabbitMQ'ya onayı gönder (BasicAck)
                // Bu, mesajın kuyruktan güvenli bir şekilde silinmesini sağlar.
                channel.BasicAck(ea.DeliveryTag, multiple: false);
            };

            // Her kuyruktan mesaj tüketmeye başlayın
            channel.BasicConsume(queue: createdQueueName, autoAck: false, consumer: consumer);
            channel.BasicConsume(queue: updatedQueueName, autoAck: false, consumer: consumer);
            channel.BasicConsume(queue: deletedQueueName, autoAck: false, consumer: consumer);
            channel.BasicConsume(queue: allBrandEventsQueue, autoAck: false, consumer: consumer); // Tüm mesajları dinleyen kuyruk

            Console.ReadLine(); // Konsol uygulamasının açık kalmasını sağlar
        }
    }
}