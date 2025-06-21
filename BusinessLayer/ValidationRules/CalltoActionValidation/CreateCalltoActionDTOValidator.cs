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
    public class CreateCalltoActionDTOValidator : AbstractValidator<CreateCalltoActionDTO>
    {
        public CreateCalltoActionDTOValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Başlık boş olamaz.")
                .MaximumLength(150).WithMessage("Başlık en fazla 150 karakter olabilir.")
                .MinimumLength(5).WithMessage("Başlık en az 5 karakter olmalıdır.");

            RuleFor(x => x.SmallTitle)
                .NotEmpty().WithMessage("Küçük Başlık boş olamaz.")
                .MaximumLength(250).WithMessage("Küçük Başlık en fazla 250 karakter olabilir.")
                .MinimumLength(10).WithMessage("Küçük Başlık en az 10 karakter olmalıdır.");

            RuleFor(x => x.ImageFile)
                .NotNull().WithMessage("Bir resim dosyası yüklemek zorunludur.")
                .Must(BeAValidImage).WithMessage("Sadece JPG, JPEG, PNG veya GIF formatında resimler yükleyebilirsiniz.")
                .Must(BeWithinFileSizeLimit).WithMessage("Resim boyutu en fazla 5 MB olmalıdır.");
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
