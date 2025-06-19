using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.BannerDTO
{
    public class CreateBannerDTO
    {
        public string SmallTitle { get; set; } = string.Empty;
        public string SubTitle { get; set; } = string.Empty;
        public IFormFile? CarImage { get; set; } // For uploading the car image
        public string CarModel { get; set; } = string.Empty;
        public string Month { get; set; } = string.Empty;
        public IFormFile? LogoImage { get; set; } // For uploading the logo image
        public string Price { get; set; } = string.Empty;
    }
}
