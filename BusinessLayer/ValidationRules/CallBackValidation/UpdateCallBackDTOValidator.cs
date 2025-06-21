using DTOsLayer.WebUIDTO.CallBackDTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ValidationRules.CallBackValidation
{
    public class UpdateCallBackDTOValidator : AbstractValidator<UpdateCallBackDTO>
    {
        public UpdateCallBackDTOValidator()
        {

            RuleFor(x => x.NameSurname)
                .NotEmpty().WithMessage("Ad Soyad boş olamaz.")
                .MaximumLength(100).WithMessage("Ad Soyad en fazla 100 karakter olabilir.")
                .MinimumLength(3).WithMessage("Ad Soyad en az 3 karakter olmalıdır.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta boş olamaz.")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Telefon Numarası boş olamaz.")
                .Matches(@"^\+?\d{10,15}$").WithMessage("Geçerli bir telefon numarası giriniz. (Örn: +905xxxxxxxxx veya 05xxxxxxxxx)");
        }
    }
}
