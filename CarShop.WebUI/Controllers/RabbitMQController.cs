using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.BrandDTO.Messages;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.WebUI.Controllers
{
    public class RabbitMQController : Controller
    {
        private readonly RabbitMQConsumerService _consumerService;

        public RabbitMQController(RabbitMQConsumerService consumerService)
        {
            _consumerService = consumerService;
        }

        public IActionResult Messages()
        {
            // DEBUG: Console'a debug bilgilerini yazdır
            Console.WriteLine($"=== RabbitMQ DEBUG ===");
            Console.WriteLine($"Toplam mesaj sayısı: {RabbitMQConsumerService.ConsumedMessages.Count}");

            // Tüm mesajları logla
            var allMessages = RabbitMQConsumerService.ConsumedMessages.ToList();
            for (int i = 0; i < allMessages.Count; i++)
            {
                var msg = allMessages[i];
                Console.WriteLine($"Mesaj {i + 1}: Type = {msg.GetType().Name}, Content = {System.Text.Json.JsonSerializer.Serialize(msg)}");
            }

            // Mesajları tipine göre ayır
            var createdMessages = new List<BrandCreatedMessage>();
            var updatedMessages = new List<BrandUpdatedMessage>();
            var deletedMessages = new List<BrandDeletedMessage>();

            foreach (var message in allMessages)
            {
                Console.WriteLine($"İşlenen mesaj tipi: {message.GetType().Name}");

                switch (message)
                {
                    case BrandCreatedMessage created:
                        createdMessages.Add(created);
                        Console.WriteLine($"Created mesaj eklendi: ID={created.BrandId}, Name={created.BrandName}");
                        break;
                    case BrandUpdatedMessage updated:
                        updatedMessages.Add(updated);
                        Console.WriteLine($"Updated mesaj eklendi: ID={updated.BrandId}, Name={updated.BrandName}");
                        break;
                    case BrandDeletedMessage deleted:
                        deletedMessages.Add(deleted);
                        Console.WriteLine($"Deleted mesaj eklendi: ID={deleted.BrandId}");
                        break;
                    default:
                        Console.WriteLine($"Bilinmeyen mesaj tipi: {message.GetType().Name}");
                        break;
                }
            }

            Console.WriteLine($"Ayrıştırma sonucu - Created: {createdMessages.Count}, Updated: {updatedMessages.Count}, Deleted: {deletedMessages.Count}");
            Console.WriteLine($"=== DEBUG SON ===");

            ViewBag.BrandCreatedMessages = createdMessages;
            ViewBag.BrandUpdatedMessages = updatedMessages;
            ViewBag.BrandDeletedMessages = deletedMessages;

            // DEBUG bilgilerini de View'a gönder
            ViewBag.TotalMessagesInMemory = allMessages.Count;
            ViewBag.AllMessages = allMessages;

            return View();
        }

        [HttpPost]
        public IActionResult ClearMessages()
        {
            var messageCount = RabbitMQConsumerService.ConsumedMessages.Count;
            RabbitMQConsumerService.ConsumedMessages.Clear();

            TempData["Message"] = $"{messageCount} mesaj temizlendi.";
            Console.WriteLine($"Toplam {messageCount} mesaj temizlendi.");

            return RedirectToAction("Messages");
        }

        // Yeni debug action
        public IActionResult DebugInfo()
        {
            var debugInfo = new
            {
                TotalMessages = RabbitMQConsumerService.ConsumedMessages.Count,
                Messages = RabbitMQConsumerService.ConsumedMessages.ToList(),
                ConsumerServiceExists = _consumerService != null,
                CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            return Json(debugInfo);
        }

        // Manuel olarak bir test mesajı ekle 
        [HttpPost]
        public IActionResult AddTestMessage()
        {
            var testMessage = new BrandCreatedMessage
            {
                BrandId = 999,
                BrandName = "Test Brand " + DateTime.Now.ToString("HH:mm:ss")
            };

            RabbitMQConsumerService.ConsumedMessages.Add(testMessage);

            TempData["Message"] = "Test mesajı eklendi.";
            Console.WriteLine($"Test mesajı eklendi: {System.Text.Json.JsonSerializer.Serialize(testMessage)}");

            return RedirectToAction("Messages");
        }
    }
}