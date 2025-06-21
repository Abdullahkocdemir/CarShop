using DTOsLayer.WebUIDTO.TestimonialDTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ValidationRules.TestimonialValidation
{
    public class CreateTestimonialDTOValidator : AbstractValidator<CreateTestimonialDTO>
    {
        public CreateTestimonialDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("İsim boş olamaz.")
                .MaximumLength(100).WithMessage("İsim en fazla 100 karakter olabilir.")
                .MinimumLength(3).WithMessage("İsim en az 3 karakter olmalıdır.");

            RuleFor(x => x.Duty)
                .NotEmpty().WithMessage("Görev boş olamaz.")
                .MaximumLength(150).WithMessage("Görev en fazla 150 karakter olabilir.")
                .MinimumLength(5).WithMessage("Görev en az 5 karakter olmalıdır.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Açıklama boş olamaz.")
                .MaximumLength(1000).WithMessage("Açıklama en fazla 1000 karakter olabilir.")
                .MinimumLength(20).WithMessage("Açıklama en az 20 karakter olmalıdır.");
        }
    }
}
