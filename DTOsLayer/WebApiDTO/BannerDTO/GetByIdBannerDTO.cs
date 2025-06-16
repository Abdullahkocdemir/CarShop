using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.BannerDTO
{
    public class GetByIdBannerDTO
    {
        public int BannerId { get; set; }
        public string SmallTitle { get; set; } = string.Empty;
        public string SubTitle { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
    }
}
