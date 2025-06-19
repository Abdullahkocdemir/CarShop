using Microsoft.AspNetCore.Http; // IFormFile için bu using gereklidir
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.FeatureDTO
{
    public class UpdateFeatureDTO
    {
        public int FeatureId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string SmallTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        // Güncelleme sırasında yüklenecek yeni resim dosyaları.
        public List<IFormFile>? NewImageFiles { get; set; }
        // Kaldırılacak mevcut resimlerin ID'leri.
        public List<int>? ImageIdsToRemove { get; set; }
    }
}