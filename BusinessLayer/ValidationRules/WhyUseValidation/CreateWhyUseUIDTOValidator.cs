using DTOsLayer.WebUIDTO.WhyUseDTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ValidationRules.WhyUseValidation
{
    public class CreateWhyUseUIDTOValidator : AbstractValidator<CreateWhyUseDTO>
    {
        public CreateWhyUseUIDTOValidator()
        {
            RuleFor(x => x.MainTitle)
                .NotEmpty().WithMessage("Ana Başlık alanı boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Ana Başlık en fazla 100 karakter olabilir.");

            RuleFor(x => x.MainDescription)
                .NotEmpty().WithMessage("Ana Açıklama alanı boş bırakılamaz.")
                .MaximumLength(500).WithMessage("Ana Açıklama en fazla 500 karakter olabilir.");

            RuleFor(x => x.VideoUrl)
                .MaximumLength(200).WithMessage("Video URL'si en fazla 200 karakter olabilir.")
                .Matches(@"^(https?://)?(www\.)?([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}(/\S*)?$")
                .When(x => !string.IsNullOrEmpty(x.VideoUrl))
                .WithMessage("Geçerli bir Video URL'si girin.");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(x => x.Content)
                    .NotEmpty().WithMessage("Alt Madde içeriği boş bırakılamaz.")
                    .MaximumLength(200).WithMessage("Alt Madde içeriği en fazla 200 karakter olabilir.");
            }).When(x => x.Items != null);

        }
    }
}
