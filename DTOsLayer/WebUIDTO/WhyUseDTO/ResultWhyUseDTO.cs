using DTOsLayer.WebUIDTO.WhyUseReasonDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.WhyUseDTO
{
    public class ResultWhyUseUIDTO
    {
        public int WhyUseId { get; set; }
        public string MainTitle { get; set; } = string.Empty;
        public string MainDescription { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public ICollection<ResultWhyUseReasonDTO>? WhyUseReasons { get; set; } // İlişkili nedenler
    }
}
