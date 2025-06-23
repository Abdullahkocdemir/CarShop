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
    public class UpdateBlogDTOValidator : AbstractValidator<UpdateBlogDTO>
    {
        public UpdateBlogDTOValidator()
        {
            RuleFor(x => x.BlogId)
                .GreaterThan(0).WithMessage("Blog ID geçerli bir değer olmalıdır.");

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

            RuleFor(x => x.BannerImage)
                .Must(BeAValidImage).When(x => x.BannerImage != null)
                .WithMessage("Banner resmi geçerli bir görsel formatında (JPG, PNG, JPEG) olmalı ve boyutu 5MB'ı geçmemelidir.");

            RuleFor(x => x.MainImage)
                .Must(BeAValidImage).When(x => x.MainImage != null)
                .WithMessage("Ana resim geçerli bir görsel formatında (JPG, PNG, JPEG) olmalı ve boyutu 5MB'ı geçmemelidir.");
        }

        private bool BeAValidImage(IFormFile? file)
        {
            if (file == null) return true; 

            var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/jpg" };
            if (!allowedContentTypes.Contains(file.ContentType))
            {
                return false;
            }

            if (file.Length > 5 * 1024 * 1024) 
            {
                return false;
            }

            return true;
        }
    }
}
