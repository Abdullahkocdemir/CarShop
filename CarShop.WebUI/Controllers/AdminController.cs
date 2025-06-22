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
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult RabbitMQMessages()
        {
            var messageCounts = _consumerService.GetAllEntityMessageCounts();
            return View(messageCounts);
        }
        [HttpGet]
        public JsonResult GetMessageCounts()
        {
            var messageCounts = _consumerService.GetAllEntityMessageCounts();
            return Json(messageCounts);
        }
        [HttpGet]
        public JsonResult GetAllMessages()
        {
            var allMessages = EnhancedRabbitMQConsumerService.AllMessages.ToList();

            var recentMessages = allMessages
                .OrderByDescending(m => m.Timestamp)
                .Take(50)
                .Select(m => new
                {
                    EntityType = m.EntityType,
                    Operation = m.Operation,
                    Timestamp = m.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                    Message = m.ToString()
                })
                .ToList();

            return Json(recentMessages);
        }
        [HttpGet]
        public JsonResult GetEntityMessages(string entityType)
        {
            string lowerCaseEntityType = entityType.ToLower();

            if (EnhancedRabbitMQConsumerService.MessagesByEntity.TryGetValue(lowerCaseEntityType, out var messages))
            {
                var recentMessages = messages
                    .OrderByDescending(m => m.Timestamp)
                    .Take(20)
                    .Select(m => new
                    {
                        EntityType = m.EntityType,
                        Operation = m.Operation,
                        Timestamp = m.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")
                    })
                    .ToList();

                return Json(recentMessages);
            }

            return Json(new List<object>());
        }
    }
}