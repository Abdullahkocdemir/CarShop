using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.RabbitMQ
{
    public abstract class BaseMessage
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string EntityType { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
    }

    public class EntityCreatedMessage<T> : BaseMessage
    {

        public required T Entity { get; set; }

        public EntityCreatedMessage()
        {
            Operation = "Created";
        }
    }

    public class EntityUpdatedMessage<T> : BaseMessage
    {
        public required T Entity { get; set; }

        public EntityUpdatedMessage()
        {
            Operation = "Updated";
        }
    }

    public class EntityDeletedMessage : BaseMessage
    {
        public int EntityId { get; set; }
        public EntityDeletedMessage()
        {
            Operation = "Deleted";
        }
    }
}