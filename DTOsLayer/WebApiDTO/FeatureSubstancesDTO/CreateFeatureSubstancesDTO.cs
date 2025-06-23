using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.FeatureSubstancesDTO
{
    public class CreateFeatureSubstancesDTO
    {
        [Required(ErrorMessage = "Konu alanı boş bırakılamaz.")]
        [StringLength(100, ErrorMessage = "Konu en fazla 100 karakter olabilir.")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Resim dosyası gereklidir.")]
        public IFormFile? ImageFile { get; set; }
    }
}
