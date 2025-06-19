using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.WhyUseReasonDTO
{
    public class CreateWhyUseReasonDTO
    {
        public string ReasonText { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public string IconCssClass { get; set; } = "fa fa-check-circle";
        // WhyUseId burada doğrudan kullanılmayacak, ana WhyUse DTO'sundan ilişkilendirilecek
    }
}
