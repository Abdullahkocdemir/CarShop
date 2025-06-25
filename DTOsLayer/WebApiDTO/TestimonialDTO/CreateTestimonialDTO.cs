using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.TestimonialDTO
{
    public class CreateTestimonialDTO
    {
        [Required(ErrorMessage = "Ad Soyad alanı boş bırakılamaz.")]
        public string NameSurname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Görev alanı boş bırakılamaz.")]
        public string Duty { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama alanı boş bırakılamaz.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Resim dosyası seçilmelidir.")]
        public IFormFile? ImageFile { get; set; } // For file upload
    }
}
