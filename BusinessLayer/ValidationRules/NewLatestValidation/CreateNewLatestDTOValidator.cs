using DTOsLayer.WebUIDTO.NewLatestDTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ValidationRules.NewLatestValidation
{
    public class CreateNewLatestDTOValidator : AbstractValidator<CreateNewLatestDTO>
    {
        public CreateNewLatestDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("İsim boş olamaz.")
                .MaximumLength(100).WithMessage("İsim en fazla 100 karakter olabilir.")
                .MinimumLength(3).WithMessage("İsim en az 3 karakter olmalıdır.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Başlık boş olamaz.")
                .MaximumLength(200).WithMessage("Başlık en fazla 200 karakter olabilir.")
                .MinimumLength(5).WithMessage("Başlık en az 5 karakter olmalıdır.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Açıklama boş olamaz.")
                .MaximumLength(1000).WithMessage("Açıklama en fazla 1000 karakter olabilir.")
                .MinimumLength(20).WithMessage("Açıklama en az 20 karakter olmalıdır.");

            RuleFor(x => x.Comment)
                .MaximumLength(500).WithMessage("Yorum en fazla 500 karakter olabilir."); 

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Tarih boş olamaz.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Tarih bugünün tarihinden veya öncesinden olmalıdır.");
        }
    }
}
