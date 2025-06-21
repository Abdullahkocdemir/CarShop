using DTOsLayer.WebUIDTO.ServiceDTO;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ValidationRules.ServiceValidation
{
    public class UpdateServiceDTOValidator : AbstractValidator<UpdateServiceDTO>
    {
        private const int MaxFileSizeInMb = 3; 
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" }; 

        public UpdateServiceDTOValidator()
        {
            RuleFor(x => x.ServiceId)
                .GreaterThan(0).WithMessage("Hizmet ID'si 0'dan büyük olmalıdır.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Başlık boş olamaz.")
                .MaximumLength(150).WithMessage("Başlık en fazla 150 karakter olabilir.")
                .MinimumLength(5).WithMessage("Başlık en az 5 karakter olmalıdır.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Açıklama boş olamaz.")
                .MaximumLength(750).WithMessage("Açıklama en fazla 750 karakter olabilir.")
                .MinimumLength(20).WithMessage("Açıklama en az 20 karakter olmalıdır.");

            When(x => x.ImageFile != null, () =>
            {
                RuleFor(x => x.ImageFile)
                    .Must(BeAValidImage).WithMessage("Sadece JPG, JPEG, PNG veya GIF formatında resimler yükleyebilirsiniz.")
                    .Must(BeWithinFileSizeLimit).WithMessage($"Resim boyutu en fazla {MaxFileSizeInMb} MB olmalıdır.");
            });

            RuleFor(x => x)
                .Must(dto => dto.ImageFile != null || !string.IsNullOrEmpty(dto.ExistingImageUrl))
                .WithMessage("Bir resim dosyası yüklemeli veya mevcut resim URL'si bulunmalıdır.")
                .When(dto => dto.ImageFile == null); 
        }

        private bool BeAValidImage(IFormFile? file)
        {
            if (file == null) return true;
            var fileExtension = System.IO.Path.GetExtension(file.FileName)?.ToLowerInvariant();
            return fileExtension != null && AllowedExtensions.Contains(fileExtension);
        }

        private bool BeWithinFileSizeLimit(IFormFile? file)
        {
            if (file == null) return true;
            const long maxBytes = (long)MaxFileSizeInMb * 1024 * 1024; 
            return file.Length <= maxBytes;
        }
    }
}
