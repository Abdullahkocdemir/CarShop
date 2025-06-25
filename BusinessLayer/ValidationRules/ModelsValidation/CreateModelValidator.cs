using DTOsLayer.WebUIDTO.ModelsDTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ValidationRules.ModelsValidation
{
    public class CreateModelValidator : AbstractValidator<CreateModelDTO>
    {
        public CreateModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Model adı boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Model adı en fazla 100 karakter olabilir.");
        }
    }
}
