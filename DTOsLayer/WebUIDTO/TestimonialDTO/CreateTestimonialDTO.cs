using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.TestimonialDTO
{
    public class CreateTestimonialDTO
    {
        [Display(Name = "Ad Soyad")]
        public string NameSurname { get; set; } = string.Empty;

        [Display(Name = "Görev")]
        public string Duty { get; set; } = string.Empty;

        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Resim Dosyası")]
        public IFormFile? ImageFile { get; set; }
    }
}
