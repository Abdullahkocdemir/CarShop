using DTOsLayer.WebUIDTO.AboutDTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ValidationRules.AboutValidation
{
    public class UpdateAboutValidator : AbstractValidator<UpdateAboutDTO>
    {
        public UpdateAboutValidator()
        {
            RuleFor(x => x.AboutId)
                .NotEmpty().WithMessage("About ID boş bırakılamaz.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Ana Başlık alanı boş bırakılamaz.")
                .MaximumLength(150).WithMessage("Ana Başlık en fazla 150 karakter olabilir.");

            RuleFor(x => x.SmallTitle)
                .NotEmpty().WithMessage("Küçük Başlık alanı boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Küçük Başlık en fazla 100 karakter olabilir.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Açıklama alanı boş bırakılamaz.")
                .MaximumLength(1000).WithMessage("Açıklama en fazla 1000 karakter olabilir.");
        }
    }
}
