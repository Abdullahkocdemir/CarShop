using DTOsLayer.WebApiDTO.WhyUseReasonDTO;
using Microsoft.AspNetCore.Http;
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

        [Required(ErrorMessage = "Ana başlık boş bırakılamaz.")]
        [StringLength(100, ErrorMessage = "Ana başlık en fazla 100 karakter olabilir.")]
        public string MainTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama boş bırakılamaz.")]
        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string MainDescription { get; set; } = string.Empty;

        [Url(ErrorMessage = "Geçerli bir URL giriniz.")]
        [StringLength(200, ErrorMessage = "Video URL'i en fazla 200 karakter olabilir.")]
        public string VideoUrl { get; set; } = string.Empty;

        public List<UpdateWhyUseReasonDTO>? WhyUseReasons { get; set; } // Formdan gelecek nedenler
    }
}
