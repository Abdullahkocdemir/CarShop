using DTOsLayer.WebUIDTO.CalltoActionDTO;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ValidationRules.CalltoActionValidation
{
    public class UpdateCalltoActionDTOValidator : AbstractValidator<UpdateCalltoActionDTO>
    {
        public UpdateCalltoActionDTOValidator()
        {
            RuleFor(x => x.CalltoActionId)
                .GreaterThan(0).WithMessage("Call to Action ID'si 0'dan büyük olmalıdır.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Başlık boş olamaz.")
                .MaximumLength(150).WithMessage("Başlık en fazla 150 karakter olabilir.")
                .MinimumLength(5).WithMessage("Başlık en az 5 karakter olmalıdır.");

            RuleFor(x => x.SmallTitle)
                .NotEmpty().WithMessage("Küçük Başlık boş olamaz.")
                .MaximumLength(250).WithMessage("Küçük Başlık en fazla 250 karakter olabilir.")
                .MinimumLength(10).WithMessage("Küçük Başlık en az 10 karakter olmalıdır.");

            When(x => x.ImageFile != null, () =>
            {
                RuleFor(x => x.ImageFile)
                    .Must(BeAValidImage).WithMessage("Sadece JPG, JPEG, PNG veya GIF formatında resimler yükleyebilirsiniz.")
                    .Must(BeWithinFileSizeLimit).WithMessage("Resim boyutu en fazla 5 MB olmalıdır.");
            });

            RuleFor(x => x)
                .Must(dto => dto.ImageFile != null || !string.IsNullOrEmpty(dto.ExistingImageUrl))
                .WithMessage("Bir resim dosyası yüklemeli veya mevcut resim URL'si bulunmalıdır.")
                .When(dto => dto.ImageFile == null); 
        }

        private bool BeAValidImage(IFormFile? file)
        {
            if (file == null) return false;
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(fileExtension);
        }

        private bool BeWithinFileSizeLimit(IFormFile? file)
        {
            if (file == null) return false;
            const int maxBytes = 5 * 1024 * 1024; 
            return file.Length <= maxBytes;
        }
    }
}
