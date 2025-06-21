using DTOsLayer.WebUIDTO.CallBackTitleDTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ValidationRules.CallBackTitleValidation
{
    public class UpdateCallBackTitleDTOValidator : AbstractValidator<UpdateCallBackTitleDTO>
    {
        public UpdateCallBackTitleDTOValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Açıklama boş olamaz.")
                .MaximumLength(200).WithMessage("Açıklama en fazla 200 karakter olabilir.")
                .MinimumLength(5).WithMessage("Açıklama en az 5 karakter olmalıdır.");
        }
    }
}
