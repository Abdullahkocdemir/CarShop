using DTOsLayer.WebUIDTO.StaffDTO;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ValidationRules.StaffValidation
{
    public class UpdateStaffDTOValidator : AbstractValidator<UpdateStaffDTO>
    {
        private const int MaxFileSizeInMb = 4;
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };

        public UpdateStaffDTOValidator()
        {

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("İsim boş olamaz.")
                .MaximumLength(100).WithMessage("İsim en fazla 100 karakter olabilir.")
                .MinimumLength(3).WithMessage("İsim en az 3 karakter olmalıdır.");

            RuleFor(x => x.Duty)
                .NotEmpty().WithMessage("Görev boş olamaz.")
                .MaximumLength(150).WithMessage("Görev en fazla 150 karakter olabilir.")
                .MinimumLength(5).WithMessage("Görev en az 5 karakter olmalıdır.");

            When(x => x.ImageFile != null, () =>
            {
                RuleFor(x => x.ImageFile)
                    .Must(BeAValidImage).WithMessage("Sadece JPG, JPEG veya PNG formatında resimler yükleyebilirsiniz.")
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
