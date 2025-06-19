using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.BannerDTO
{
    public class UpdateBannerDTO
    {
        public int BannerId { get; set; }
        public string SmallTitle { get; set; } = string.Empty;
        public string SubTitle { get; set; } = string.Empty;
        public IFormFile? CarImage { get; set; } // Yeni araba resmi yüklemek için (isteğe bağlı)
        public string? ExistingCarImageUrl { get; set; } // Mevcut araba resmi URL'si (göstermek için)
        public string CarModel { get; set; } = string.Empty;
        public string Month { get; set; } = string.Empty;
        public IFormFile? LogoImage { get; set; } // Yeni logo resmi yüklemek için (isteğe bağlı)
        public string? ExistingLogoImageUrl { get; set; } // Mevcut logo resmi URL'si (göstermek için)
        public string Price { get; set; } = string.Empty;
    }
}
