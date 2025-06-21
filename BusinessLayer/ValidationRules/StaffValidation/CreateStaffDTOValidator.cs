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
    public class CreateStaffDTOValidator : AbstractValidator<CreateStaffDTO>
    {
        private const int MaxFileSizeInMb = 4;
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" }; 

        public CreateStaffDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("İsim boş olamaz.")
                .MaximumLength(100).WithMessage("İsim en fazla 100 karakter olabilir.")
                .MinimumLength(3).WithMessage("İsim en az 3 karakter olmalıdır.");

            RuleFor(x => x.Duty)
                .NotEmpty().WithMessage("Görev boş olamaz.")
                .MaximumLength(150).WithMessage("Görev en fazla 150 karakter olabilir.")
                .MinimumLength(5).WithMessage("Görev en az 5 karakter olmalıdır.");

            RuleFor(x => x.ImageFile)
                .NotNull().WithMessage("Bir resim dosyası yüklemek zorunludur.")
                .Must(BeAValidImage).WithMessage("Sadece JPG, JPEG veya PNG formatında resimler yükleyebilirsiniz.")
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
