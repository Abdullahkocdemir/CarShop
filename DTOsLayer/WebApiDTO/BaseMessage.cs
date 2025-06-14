using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO
{
    public abstract class BaseMessage
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string EntityType { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty; // Created, Updated, Deleted
    }
    public class EntityCreatedMessage<T> : BaseMessage
    {
        public T Entity { get; set; }
        public EntityCreatedMessage()
        {
            Operation = "Created";
        }
    }

    public class EntityUpdatedMessage<T> : BaseMessage
    {
        public T Entity { get; set; }
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
