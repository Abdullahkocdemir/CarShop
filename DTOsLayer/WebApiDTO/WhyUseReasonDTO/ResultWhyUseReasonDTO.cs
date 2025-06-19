using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.WhyUseReasonDTO
{
    public class ResultWhyUseReasonDTO
    {
        public int WhyUseReasonId { get; set; }
        public string ReasonText { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public string IconCssClass { get; set; } = "fa fa-check-circle";
        public int WhyUseId { get; set; }
    }
}
