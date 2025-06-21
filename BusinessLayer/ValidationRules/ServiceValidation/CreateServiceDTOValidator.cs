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
    public class CreateServiceDTOValidator : AbstractValidator<CreateServiceDTO>
    {
        private const int MaxFileSizeInMb = 3; 
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" }; 

        public CreateServiceDTOValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Başlık boş olamaz.")
                .MaximumLength(150).WithMessage("Başlık en fazla 150 karakter olabilir.")
                .MinimumLength(5).WithMessage("Başlık en az 5 karakter olmalıdır.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Açıklama boş olamaz.")
                .MaximumLength(750).WithMessage("Açıklama en fazla 750 karakter olabilir.")
                .MinimumLength(20).WithMessage("Açıklama en az 20 karakter olmalıdır.");

            RuleFor(x => x.ImageFile)
                .NotNull().WithMessage("Bir resim dosyası yüklemek zorunludur.")
                .Must(BeAValidImage).WithMessage("Sadece JPG, JPEG, PNG veya GIF formatında resimler yükleyebilirsiniz.")
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
