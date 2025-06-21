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
    public class UpdateFeatureDTOValidator : AbstractValidator<UpdateFeatureDTO>
    {
        private const int MaxFileSizeInMb = 5;
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

        public UpdateFeatureDTOValidator()
        {
            RuleFor(x => x.FeatureId)
                .GreaterThan(0).WithMessage("Özellik ID'si 0'dan büyük olmalıdır.");

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

            When(x => x.NewImageFiles != null && x.NewImageFiles.Any(), () =>
            {
                RuleFor(x => x.NewImageFiles)
                    .ForEach(file =>
                    {
                        file.Must(BeAValidImage).WithMessage("Yüklenen resimlerden biri geçersiz formatta (JPG, JPEG, PNG, GIF olmalı).")
                            .Must(BeWithinFileSizeLimit).WithMessage($"Yüklenen resimlerden biri {MaxFileSizeInMb} MB'tan büyük.");
                    });
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
