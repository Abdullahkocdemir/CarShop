using Microsoft.AspNetCore.Mvc;
using BusinessLayer.RabbitMQ;
using System.Linq;
using System.Collections.Generic;
using DTOsLayer.WebApiDTO.BrandDTO.Messages; // Mesaj DTO'larınızın olduğu namespace

namespace CarShop.WebUI.Controllers
{
    public class RabbitMQMessagesController : Controller
    {
        public IActionResult Index()
        {
            // Tüketilen mesajları doğrudan statik listeden alıyoruz.
            // Gerçek bir senaryoda bu veritabanından veya başka bir kalıcı kaynaktan gelmelidir.
            var messages = RabbitMQConsumerService.ConsumedMessages.ToList();

            // Mesajları görüntülemek için bir model oluşturabilirsiniz
            var brandCreatedMessages = messages.OfType<BrandCreatedMessage>().ToList();
            var brandUpdatedMessages = messages.OfType<BrandUpdatedMessage>().ToList();
            var brandDeletedMessages = messages.OfType<BrandDeletedMessage>().ToList();

            ViewBag.BrandCreatedMessages = brandCreatedMessages;
            ViewBag.BrandUpdatedMessages = brandUpdatedMessages;
            ViewBag.BrandDeletedMessages = brandDeletedMessages;

            return View(messages);
        }

        // Opsiyonel: Mesajları temizlemek için bir aksiyon
        [HttpPost]
        public IActionResult ClearMessages()
        {
            RabbitMQConsumerService.ConsumedMessages.Clear();
            TempData["Message"] = "Mesajlar temizlendi.";
            return RedirectToAction(nameof(Index));
        }
    }
}