using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class FeatureImage
    {
        public int FeatureImageId { get; set; }
        public int FeatureId { get; set; } 
        public string ImageUrl { get; set; } = string.Empty;
        public string FileName { get; set; }= string.Empty;
        public DateTime UploadDate { get; set; }

        [JsonIgnore]
        public virtual Feature? Feature { get; set; }
    }
}
