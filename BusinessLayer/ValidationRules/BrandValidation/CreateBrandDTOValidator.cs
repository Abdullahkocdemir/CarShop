using DTOsLayer.WebUIDTO.BrandDTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ValidationRules.BrandValidation
{
    public class CreateBrandDTOValidator : AbstractValidator<CreateBrandDTO>
    {
        public CreateBrandDTOValidator()
        {
            RuleFor(x => x.BrandName)
                .NotEmpty().WithMessage("Marka Adı boş olamaz.")
                .MaximumLength(50).WithMessage("Marka Adı en fazla 50 karakter olabilir.")
                .MinimumLength(2).WithMessage("Marka Adı en az 2 karakter olmalıdır.");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Ülke boş olamaz.")
                .MaximumLength(50).WithMessage("Ülke en fazla 50 karakter olabilir.");

            RuleFor(x => x.EstablishmentYear)
                .NotEmpty().WithMessage("Kuruluş Yılı boş olamaz.")
                .Length(4).WithMessage("Kuruluş Yılı 4 hane olmalıdır.")
                .Matches(@"^\d{4}$").WithMessage("Kuruluş Yılı geçerli bir 4 haneli yıl olmalıdır.");
        }
    }
}
