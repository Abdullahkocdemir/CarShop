using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.BannerDTO
{
    public class CreateBannerDTO
    {
        public string SmallTitle { get; set; } = string.Empty;
        public string SubTitle { get; set; } = string.Empty;
        public IFormFile? CarImage { get; set; } // Araba resmi yüklemek için
        public string CarModel { get; set; } = string.Empty;
        public string Month { get; set; } = string.Empty;
        public IFormFile? LogoImage { get; set; } // Logo resmi yüklemek için
        public string Price { get; set; } = string.Empty;
    }
}
