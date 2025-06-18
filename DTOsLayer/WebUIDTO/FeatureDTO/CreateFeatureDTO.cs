using Microsoft.AspNetCore.Http; // IFormFile için bu using gereklidir
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.FeatureDTO
{
    public class CreateFeatureDTO
    {
        public string Title { get; set; } = string.Empty;
        public string SmallTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        // public string ImageUrl { get; set; } = string.Empty; // API'ye gönderirken bu gerekli değil, sadece IFormFile gönderilecek
        // Eğer UI'da create sonrası URL'yi tutmak isterseniz kalabilir ama genelde gerekmez.

        // Resim dosyasını taşıyacak olan property
        public IFormFile? ImageFile { get; set; } // Nullable yapıldı, resim zorunlu değilse
    }
}