using DTOsLayer.WebUIDTO.FeatureSubstancesDTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ValidationRules.FeatureSubstanceValidation
{
    public class UpdateFeatureSubstanceDTOValidator : AbstractValidator<UpdateFeatureSubstancesDTO>
    {
        public UpdateFeatureSubstanceDTOValidator()
        {
            RuleFor(x => x.FeatureSubstanceId)
                .NotEmpty().WithMessage("Özellik kimliği boş bırakılamaz.")
                .GreaterThan(0).WithMessage("Özellik kimliği 0'dan büyük olmalıdır.");

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Konu alanı boş bırakılamaz.");

            RuleFor(x => x.Subject)
                .MaximumLength(100).WithMessage("Konu en fazla 100 karakter olabilir.");
        }
    }
}
