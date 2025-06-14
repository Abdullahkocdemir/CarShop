using BusinessLayer.RabbitMQ;
using Microsoft.AspNetCore.Mvc;
using System; // DateTime için eklendi
using System.Linq; // OrderByDescending, Select, Take için eklendi
using System.Collections.Generic; // List ve Dictionary için eklendi

namespace CarShop.WebUI.Controllers
{
    public class AdminController : Controller
    {
        private readonly EnhancedRabbitMQConsumerService _consumerService;

        public AdminController(EnhancedRabbitMQConsumerService consumerService)
        {
            _consumerService = consumerService;
        }

        /// <summary>
        /// Yönetim panosu ana sayfasını görüntüler.
        /// </summary>
        public IActionResult Dashboard()
        {
            return View();
        }

        /// <summary>
        /// RabbitMQ mesaj istatistiklerinin gösterildiği sayfayı görüntüler.
        /// </summary>
        public IActionResult RabbitMQMessages()
        {
            // Tüketici servisinden mevcut mesaj sayılarını al
            var messageCounts = _consumerService.GetAllEntityMessageCounts();
            // Bu veriyi View'a gönder
            return View(messageCounts);
        }

        /// <summary>
        /// Canlı olarak güncellenebilecek mesaj sayılarını JSON formatında döndürür.
        /// </summary>
        /// <returns>Varlık tipine göre oluşturulan, güncellenen ve silinen mesaj sayılarını içeren JSON nesnesi.</returns>
        [HttpGet]
        public JsonResult GetMessageCounts()
        {
            // Tüketici servisinden güncel mesaj sayılarını al
            var messageCounts = _consumerService.GetAllEntityMessageCounts();
            return Json(messageCounts);
        }

        /// <summary>
        /// En son 50 genel RabbitMQ mesajını JSON formatında döndürür.
        /// </summary>
        /// <returns>Son mesajların listesini içeren JSON nesnesi.</returns>
        [HttpGet]
        public JsonResult GetAllMessages()
        {
            // Tüm mesajları statik ConcurrentBag'den al
            var allMessages = EnhancedRabbitMQConsumerService.AllMessages.ToList();

            // Mesajları zaman damgasına göre azalan sırada sırala ve en son 50 tanesini al
            var recentMessages = allMessages
                .OrderByDescending(m => m.Timestamp)
                .Take(50)
                .Select(m => new
                {
                    // Mesajın EntityType, Operation ve zaman damgası bilgileri
                    EntityType = m.EntityType,
                    Operation = m.Operation,
                    Timestamp = m.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"), // Tarih formatı
                    // Mesajın kendine özgü verisini göster. BaseMessage'da ToString override edilmediği için
                    // varsayılan olarak tip adını dönebilir. Gerçek içeriği görmek için burayı geliştirebilirsin.
                    // Örneğin, EntityCreatedMessage için Entity'nin ID'sini veya adını ekleyebilirsin.
                    Message = m.ToString() // Şimdilik varsayılan ToString() metodunu kullanıyoruz
                })
                .ToList();

            return Json(recentMessages);
        }

        /// <summary>
        /// Belirli bir varlık tipine ait en son 20 RabbitMQ mesajını JSON formatında döndürür.
        /// </summary>
        /// <param name="entityType">Mesajları filtrelenecek varlık tipi (örn. "Brand", "Product").</param>
        /// <returns>Belirli varlık tipine ait son mesajların listesini içeren JSON nesnesi.</returns>
        [HttpGet]
        public JsonResult GetEntityMessages(string entityType)
        {
            // entityType parametresi büyük/küçük harf duyarlılığı olmadan işlenmeli
            string lowerCaseEntityType = entityType.ToLower();

            // ConcurrentDictionary'den belirli entityType'a ait mesajları almayı dene
            if (EnhancedRabbitMQConsumerService.MessagesByEntity.TryGetValue(lowerCaseEntityType, out var messages))
            {
                // Mesajları zaman damgasına göre azalan sırada sırala ve en son 20 tanesini al
                var recentMessages = messages
                    .OrderByDescending(m => m.Timestamp)
                    .Take(20)
                    .Select(m => new
                    {
                        // Mesajın EntityType, Operation ve zaman damgası bilgileri
                        EntityType = m.EntityType,
                        Operation = m.Operation,
                        Timestamp = m.Timestamp.ToString("yyyy-MM-dd HH:mm:ss") // Tarih formatı
                        // Burada da istersen Message içeriğini ekleyebilirsin
                    })
                    .ToList();

                return Json(recentMessages);
            }

            // Belirli entityType için mesaj bulunamazsa veya anahtar yoksa boş bir liste döndür
            return Json(new List<object>());
        }
    }
}