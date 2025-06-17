using DTOsLayer.WebUIDTO.BroadcastDTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ValidationRules.BroadcastValidation
{
    public class UpdateBroadcastDTOValidator : AbstractValidator<UpdateBroadcastDTO>
    {
        public UpdateBroadcastDTOValidator()
        {
            RuleFor(x => x.BroadcastId)
                .GreaterThan(0).WithMessage("Duyuru ID sıfırdan büyük olmalıdır.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Başlık boş olamaz.")
                .MaximumLength(100).WithMessage("Başlık en fazla 100 karakter olabilir.");

            RuleFor(x => x.SmallTitle)
                .NotEmpty().WithMessage("Küçük Başlık boş olamaz.")
                .MaximumLength(200).WithMessage("Küçük Başlık en fazla 200 karakter olabilir.");

        }
    }
}
