using DTOsLayer.WebUIDTO.WhyUseItemDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.WhyUseDTO
{
    public class ResultWhyUseDTO
    {
        public int WhyUseId { get; set; }
        public string MainTitle { get; set; } = string.Empty;
        public string MainDescription { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public List<ResultWhyUseItemDTO> Items { get; set; } = new List<ResultWhyUseItemDTO>();
    }
}
