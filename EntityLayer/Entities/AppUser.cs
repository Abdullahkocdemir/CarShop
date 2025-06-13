using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EntityLayer.Entities
{
    public class AppUser: IdentityUser<string>
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)] // ObjectId olarak temsil et
        public string? Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
