using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.BannerDTO
{
    public class ResultBannerDTO
    {
        public int BannerId { get; set; }
        public string SmallTitle { get; set; } = string.Empty;
        public string SubTitle { get; set; } = string.Empty;
        public string CarImageUrl { get; set; } = string.Empty; // API'dan tam URL gelecek
        public string CarModel { get; set; } = string.Empty;
        public string Month { get; set; } = string.Empty;
        public string LogoImageUrl { get; set; } = string.Empty; // API'dan tam URL gelecek
        public string Price { get; set; } = string.Empty;
    }
}
