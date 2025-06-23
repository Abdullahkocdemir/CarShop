using DTOsLayer.WebUIDTO.FeatureDTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ValidationRules.FeatureValidation
{
    public class CreateFeatureDTOValidator : AbstractValidator<CreateFeatureDTO>
    {
        public CreateFeatureDTOValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Başlık alanı boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Başlık en fazla 100 karakter olabilir.");

            RuleFor(x => x.SmallDescription)
                .MaximumLength(250).WithMessage("Kısa açıklama en fazla 250 karakter olabilir.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Açıklama alanı boş bırakılamaz.");
        }
    }
}
