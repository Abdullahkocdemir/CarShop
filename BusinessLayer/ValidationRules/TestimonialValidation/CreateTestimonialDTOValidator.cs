using DTOsLayer.WebUIDTO.TestimonialDTO;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ValidationRules.TestimonialValidation
{
    public class CreateTestimonialDTOValidator : AbstractValidator<CreateTestimonialDTO>
    {
        public CreateTestimonialDTOValidator()
        {
            RuleFor(x => x.NameSurname)
                .NotEmpty().WithMessage("Ad Soyad alanı boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Ad Soyad alanı en fazla 100 karakter olabilir.");

            RuleFor(x => x.Duty)
                .NotEmpty().WithMessage("Görev alanı boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Görev alanı en fazla 100 karakter olabilir.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Açıklama alanı boş bırakılamaz.")
                .MaximumLength(500).WithMessage("Açıklama alanı en fazla 500 karakter olabilir.");

            RuleFor(x => x.ImageFile)
                .NotNull().WithMessage("Resim dosyası yüklenmelidir.")
                .Must(BeAValidImage).WithMessage("Lütfen geçerli bir resim dosyası seçin (JPG, JPEG, PNG, GIF).");
        }

        private bool BeAValidImage(IFormFile? file)
        {
            if (file == null) return false;
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            return allowedExtensions.Contains(fileExtension);
        }
    }
}
