using DTOsLayer.WebApiDTO.WhyUseItemDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.WhyUseDTO
{
    public class CreateWhyUseDTO
    {
        public string MainTitle { get; set; } = string.Empty;
        public string MainDescription { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public List<CreateWhyUseItemDTO> Items { get; set; } = new List<CreateWhyUseItemDTO>();
    }
}
