using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.TestimonialDTO
{
    public class UpdateTestimonialDTO
    {
        public int TestimonialId { get; set; }

        [Display(Name = "Ad Soyad")]
        public string NameSurname { get; set; } = string.Empty;

        [Display(Name = "Görev")]
        public string Duty { get; set; } = string.Empty;

        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Yeni Resim")]
        public IFormFile? ImageFile { get; set; }

        public string? ExistingImageUrl { get; set; }
    }
}
