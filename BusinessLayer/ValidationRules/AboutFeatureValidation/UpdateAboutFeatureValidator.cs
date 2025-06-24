using DTOsLayer.WebUIDTO.AboutFeatureDTO;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ValidationRules.AboutFeatureValidation
{
    public class UpdateAboutFeatureValidator : AbstractValidator<UpdateAboutFeatureDTO>
    {
        public UpdateAboutFeatureValidator()
        {
            RuleFor(x => x.AboutFeatureId)
                .NotEmpty().WithMessage("AboutFeature ID boş bırakılamaz.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Başlık alanı boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Başlık en fazla 100 karakter olabilir.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Açıklama alanı boş bırakılamaz.")
                .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.");

            When(x => x.ImageFile == null, () =>
            {
                RuleFor(x => x.ExistingImageUrl)
                    .NotEmpty().WithMessage("Resim dosyası seçilmelidir veya mevcut resim URL'si bulunmalıdır.");
            });
        }
    }
}
