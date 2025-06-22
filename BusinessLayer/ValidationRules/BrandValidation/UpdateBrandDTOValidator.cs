using DTOsLayer.WebUIDTO.BrandDTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ValidationRules.BrandValidation
{
    public class UpdateBrandDTOValidator : AbstractValidator<UpdateBrandDTO>
    {
        public UpdateBrandDTOValidator()
        {
            RuleFor(x => x.BrandName)
                .NotEmpty().WithMessage("Marka Adı boş olamaz.")
                .MaximumLength(50).WithMessage("Marka Adı en fazla 50 karakter olabilir.")
                .MinimumLength(2).WithMessage("Marka Adı en az 2 karakter olmalıdır.");
        }
    }
}
