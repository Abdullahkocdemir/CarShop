using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.WhyUseReasonDTO
{
    public class ResultWhyUseReasonDTO
    {
        public int WhyUseReasonId { get; set; }
        public string ReasonText { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public string IconCssClass { get; set; } = "fa fa-check-circle";
        // WhyUseId'yi burada UI'da doğrudan kullanmıyor olabiliriz, ama gerekirse eklenebilir.
    }
}
