using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.WhyUseItemDTO
{
    public class UpdateWhyUseItemDTO
    {
        public int Id { get; set; } 

        [Required(ErrorMessage = "İçerik alanı boş bırakılamaz.")]
        [StringLength(200, ErrorMessage = "İçerik en fazla 200 karakter olabilir.")]
        public string Content { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } 
    }
}
