using DTOsLayer.WebUIDTO.FeatureSubstancesDTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ValidationRules.FeatureSubstanceValidation
{
    public class CreateFeatureSubstanceDTOValidator : AbstractValidator<CreateFeatureSubstancesDTO>
    {
        public CreateFeatureSubstanceDTOValidator()
        {
            RuleFor(x => x.Subject).NotEmpty().WithMessage("Konu alanı boş bırakılamaz.");
            RuleFor(x => x.Subject).MaximumLength(100).WithMessage("Konu en fazla 100 karakter olabilir.");
            RuleFor(x => x.ImageFile).NotNull().WithMessage("Resim dosyası gereklidir.");
        }
    }
}
