using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.WhyUseReasonDTO
{
    public class UpdateWhyUseReasonDTO
    {
        public int WhyUseReasonId { get; set; } // Güncelleme için ID gerekli

        [Required(ErrorMessage = "Neden metni boş bırakılamaz.")]
        [StringLength(250, ErrorMessage = "Neden metni en fazla 250 karakter olabilir.")]
        public string ReasonText { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sıralama numarası boş bırakılamaz.")]
        [Range(1, 100, ErrorMessage = "Sıralama numarası 1 ile 100 arasında olmalıdır.")]
        public int DisplayOrder { get; set; }

        public string IconCssClass { get; set; } = "fa fa-check-circle"; // Varsayılan değer
        public int WhyUseId { get; set; } // Hangi WhyUse'a ait olduğunu belirtmek için
    }
}
