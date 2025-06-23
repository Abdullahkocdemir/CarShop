using Microsoft.AspNetCore.Http; // IFormFile için bu using gereklidir
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.FeatureDTO
{
    public class CreateFeatureDTO
    {
        [Required(ErrorMessage = "Başlık alanı boş bırakılamaz.")]
        [StringLength(100, ErrorMessage = "Başlık en fazla 100 karakter olabilir.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "Kısa açıklama en fazla 250 karakter olabilir.")]
        public string SmallDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama alanı boş bırakılamaz.")]
        [DataType(DataType.MultilineText)] 
        public string Description { get; set; } = string.Empty;
    }
}