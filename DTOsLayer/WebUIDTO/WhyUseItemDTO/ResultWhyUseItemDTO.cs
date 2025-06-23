using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.WhyUseItemDTO
{
    public class ResultWhyUseItemDTO
    {
        public int Id { get; set; } // İlişkili öğenin ID'si (güncelleme için gerekli)
        public string Content { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } // Silinecek öğeleri işaretlemek için
    }
}
