using DTOsLayer.WebUIDTO.WhyUseItemDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.WhyUseDTO
{
    public class UpdateWhyUseDTO
    {
        public int WhyUseId { get; set; }

        [Required(ErrorMessage = "Ana Başlık alanı boş bırakılamaz.")]
        [StringLength(100, ErrorMessage = "Ana Başlık en fazla 100 karakter olabilir.")]
        public string MainTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ana Açıklama alanı boş bırakılamaz.")]
        [StringLength(500, ErrorMessage = "Ana Açıklama en fazla 500 karakter olabilir.")]
        public string MainDescription { get; set; } = string.Empty;

        [Url(ErrorMessage = "Geçerli bir Video URL'si girin.")]
        [StringLength(200, ErrorMessage = "Video URL'si en fazla 200 karakter olabilir.")]
        public string VideoUrl { get; set; } = string.Empty;

        // WhyUseItem'lar için koleksiyon
        public List<UpdateWhyUseItemDTO> Items { get; set; } = new List<UpdateWhyUseItemDTO>();
    }
}
