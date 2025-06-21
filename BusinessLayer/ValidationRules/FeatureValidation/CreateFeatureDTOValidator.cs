using DTOsLayer.WebUIDTO.FeatureDTO;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ValidationRules.FeatureValidation
{
    public class CreateFeatureDTOValidator : AbstractValidator<CreateFeatureDTO>
    {
        private const int MaxFileSizeInMb = 5; 
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" }; 

        public CreateFeatureDTOValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Başlık boş olamaz.")
                .MaximumLength(100).WithMessage("Başlık en fazla 100 karakter olabilir.")
                .MinimumLength(3).WithMessage("Başlık en az 3 karakter olmalıdır.");

            RuleFor(x => x.SmallTitle)
                .NotEmpty().WithMessage("Küçük Başlık boş olamaz.")
                .MaximumLength(200).WithMessage("Küçük Başlık en fazla 200 karakter olabilir.")
                .MinimumLength(5).WithMessage("Küçük Başlık en az 5 karakter olmalıdır.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Açıklama boş olamaz.")
                .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.")
                .MinimumLength(10).WithMessage("Açıklama en az 10 karakter olmalıdır.");

            RuleFor(x => x.ImageFiles)
                .NotNull().WithMessage("En az bir resim dosyası yüklemek zorunludur.")
                .Must(list => list != null && list.Any()).WithMessage("En az bir resim dosyası yüklemek zorunludur.")
                .ForEach(file => 
                {
                    file.Must(BeAValidImage).WithMessage("Sadece JPG, JPEG, PNG veya GIF formatında resimler yükleyebilirsiniz.")
                        .Must(BeWithinFileSizeLimit).WithMessage($"Resim boyutu en fazla {MaxFileSizeInMb} MB olmalıdır.");
                });
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
