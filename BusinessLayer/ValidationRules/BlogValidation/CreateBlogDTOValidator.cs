using DTOsLayer.WebUIDTO.BlogDTO;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ValidationRules.BlogValidation
{
    public class CreateBlogDTOValidator : AbstractValidator<CreateBlogDTO>
    {
        public CreateBlogDTOValidator()
        {
            RuleFor(x => x.SmallTitle)
                .NotEmpty().WithMessage("Küçük Başlık alanı boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Küçük Başlık en fazla 100 karakter olabilir.");

            RuleFor(x => x.Author)
                .NotEmpty().WithMessage("Yazar alanı boş bırakılamaz.")
                .MaximumLength(50).WithMessage("Yazar en fazla 50 karakter olabilir.");

            RuleFor(x => x.SmallDescription)
                .NotEmpty().WithMessage("Küçük Açıklama alanı boş bırakılamaz.")
                .MaximumLength(250).WithMessage("Küçük Açıklama en fazla 250 karakter olabilir.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Başlık alanı boş bırakılamaz.")
                .MaximumLength(200).WithMessage("Başlık en fazla 200 karakter olabilir.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Açıklama alanı boş bırakılamaz.");

        }

    }
}
