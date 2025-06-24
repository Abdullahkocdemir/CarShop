using DTOsLayer.WebUIDTO.ShowroomDTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ValidationRules.ShowroomValidation
{
    public class CreateShowroomValidator : AbstractValidator<CreateShowroomDTO>
    {
        public CreateShowroomValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Başlık boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Başlık en fazla 100 karakter olabilir.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Adres boş bırakılamaz.")
                .MaximumLength(250).WithMessage("Adres en fazla 250 karakter olabilir.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Telefon numarası boş bırakılamaz.")
                .Matches(@"^\+?\d{10,15}$").WithMessage("Geçerli bir telefon numarası giriniz (örn: +905xxxxxxxxx veya 05xxxxxxxxx).");
        }
    }
}
