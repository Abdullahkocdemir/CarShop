using DTOsLayer.WebUIDTO.PartnerDTO;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ValidationRules.PartnerValidation
{
    public class UpdatePartnerDTOValidator : AbstractValidator<UpdatePartnerDTO>
    {
        private const int MaxFileSizeInMb = 2;
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".svg" };

        public UpdatePartnerDTOValidator()
        {
            RuleFor(x => x.PartnerId)
                .GreaterThan(0).WithMessage("Ortak ID'si 0'dan büyük olmalıdır.");

            When(x => x.ImageFile != null, () =>
            {
                RuleFor(x => x.ImageFile)
                    .Must(BeAValidImage).WithMessage("Sadece JPG, JPEG, PNG, GIF veya SVG formatında resimler yükleyebilirsiniz.")
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
