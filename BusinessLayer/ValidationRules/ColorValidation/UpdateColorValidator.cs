using DTOsLayer.WebUIDTO.ColorDTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ValidationRules.ColorValidation
{
    public class UpdateColorValidator : AbstractValidator<UpdateColorDTO>
    {
        public UpdateColorValidator()
        {
            RuleFor(x => x.ColorId)
                .NotEmpty().WithMessage("Renk ID boş bırakılamaz.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Renk adı boş bırakılamaz.")
                .MaximumLength(50).WithMessage("Renk adı en fazla 50 karakter olabilir.");
        }
    }
}
