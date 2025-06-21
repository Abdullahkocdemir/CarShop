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
    public class CreatePartnerDTOValidator : AbstractValidator<CreatePartnerDTO>
    {
        private const int MaxFileSizeInMb = 2;
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".svg" };

        public CreatePartnerDTOValidator()
        {
            RuleFor(x => x.ImageFile)
                .NotNull().WithMessage("Bir ortak logosu yüklemek zorunludur.")
                .Must(BeAValidImage).WithMessage("Sadece JPG, JPEG, PNG, GIF veya SVG formatında resimler yükleyebilirsiniz.")
                .Must(BeWithinFileSizeLimit).WithMessage($"Resim boyutu en fazla {MaxFileSizeInMb} MB olmalıdır.");
        }

        private bool BeAValidImage(IFormFile? file)
        {
            if (file == null) return false;
            var fileExtension = System.IO.Path.GetExtension(file.FileName)?.ToLowerInvariant();
            return fileExtension != null && AllowedExtensions.Contains(fileExtension);
        }

        private bool BeWithinFileSizeLimit(IFormFile? file)
        {
            if (file == null) return false;
            const long maxBytes = (long)MaxFileSizeInMb * 1024 * 1024;
            return file.Length <= maxBytes;
        }
    }
}
