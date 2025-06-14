using BusinessLayer.RabbitMQ;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.WebAPI.Controllers
{
    public abstract class BaseEntityController : ControllerBase
    {
        protected readonly EnhancedRabbitMQService _rabbitMqService;
        protected abstract string EntityTypeName { get; }

        public BaseEntityController(EnhancedRabbitMQService rabbitMqService)
        {
            _rabbitMqService = rabbitMqService;
        }

        protected void PublishEntityCreated<T>(T entity)
        {
            _rabbitMqService.PublishEntityMessage(entity, "Created", EntityTypeName);
        }

        protected void PublishEntityUpdated<T>(T entity)
        {
            _rabbitMqService.PublishEntityMessage(entity, "Updated", EntityTypeName);
        }

        protected void PublishEntityDeleted<T>(T entity)
        {
            _rabbitMqService.PublishEntityMessage(entity, "Deleted", EntityTypeName);
        }
    }
}
