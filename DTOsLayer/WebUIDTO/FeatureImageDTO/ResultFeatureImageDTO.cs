using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.FeatureImageDTO
{
    public class ResultFeatureImageDTO
    {
        public int FeatureImageId { get; set; }
        public int FeatureId { get; set; } // Hangi özelliğe ait olduğunu belirtir
        public string ImageUrl { get; set; } = string.Empty; // Resmin URL'si
        public string FileName { get; set; } = string.Empty; // Resmin orijinal dosya adı
    }
}
