using AutoMapper;
using DTOsLayer.WebUIDTO.BannerDTO;

namespace CarShop.WebUI.Mapping
{
    public class GeneralMapping : Profile
    {
        public GeneralMapping()
        {
            // API'dan gelen ResultBannerDTO'yu UI'da kullanılacak ResultBannerUIDTO'ya eşleme
            // Bu iki DTO'nun yapısı zaten aynı olduğu için doğrudan eşleyebiliriz.
            CreateMap<DTOsLayer.WebApiDTO.BannerDTO.ResultBannerDTO, ResultBannerDTO>().ReverseMap();

            // API'dan gelen ResultBannerDTO'yu, Güncelleme sayfasında doldurmak için UpdateBannerUIDTO'ya eşleme
            CreateMap<DTOsLayer.WebApiDTO.BannerDTO.ResultBannerDTO, UpdateBannerDTO>()
                .ForMember(dest => dest.CarImage, opt => opt.Ignore()) // UI tarafında dosya yükleme için, ignore ediyoruz
                .ForMember(dest => dest.LogoImage, opt => opt.Ignore()) // UI tarafında dosya yükleme için, ignore ediyoruz
                .ForMember(dest => dest.ExistingCarImageUrl, opt => opt.MapFrom(src => src.CarImageUrl)) // Mevcut URL'yi Existing alanına atıyoruz
                .ForMember(dest => dest.ExistingLogoImageUrl, opt => opt.MapFrom(src => src.LogoImageUrl)); // Mevcut URL'yi Existing alanına atıyoruz

            // CreateBannerUIDTO'dan Web API'nin CreateBannerDTO'suna eşleme
            // Bu eşleme, Web UI'dan API'ya veri gönderirken kullanılır.
            // Dosya alanları (CarImage, LogoImage) API tarafında MultipartFormDataContent ile manuel eklendiği için burada ignore ediyoruz.
            CreateMap<CreateBannerDTO, DTOsLayer.WebApiDTO.BannerDTO.CreateBannerDTO>()
                .ForMember(dest => dest.CarImage, opt => opt.Ignore())
                .ForMember(dest => dest.LogoImage, opt => opt.Ignore());

            // UpdateBannerUIDTO'dan Web API'nin UpdateBannerDTO'suna eşleme
            // Bu eşleme, Web UI'dan API'ya veri gönderirken kullanılır.
            // Dosya ve ExistingImageUrl alanları API tarafında manuel eklendiği/işlendiği için burada ignore ediyoruz.
            CreateMap<UpdateBannerDTO, DTOsLayer.WebApiDTO.BannerDTO.UpdateBannerDTO>()
                .ForMember(dest => dest.CarImage, opt => opt.Ignore())
                .ForMember(dest => dest.LogoImage, opt => opt.Ignore())
                .ForMember(dest => dest.ExistingCarImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.ExistingLogoImageUrl, opt => opt.Ignore());




            // ProductImageDTO eşlemesi (hem WebAPI hem WebUI arasında kullanılacak)
            // Eğer Web API'den gelen ProductImageDTO'nun ID'si varsa kullanın
            CreateMap<DTOsLayer.WebApiDTO.ProductDTOs.ProductImageDTO, DTOsLayer.WebUIDTO.ProductDTO.ProductImageDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) // Eğer WebAPI'deki ProductImageDTO'da Id varsa bunu kullanır
                .ReverseMap();

            // WebApi ResultProductDTO'dan WebUI ResultProductDTO'ya
            CreateMap<DTOsLayer.WebApiDTO.ProductDTOs.ResultProductDTO, DTOsLayer.WebUIDTO.ProductDTO.ResultProductDTO>()
                 .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls))
                 .ReverseMap();

            // WebApi GetByIdProductDTO'dan WebUI GetByIdProductDTO'ya
            CreateMap<DTOsLayer.WebApiDTO.ProductDTOs.GetByIdProductDTO, DTOsLayer.WebUIDTO.ProductDTO.GetByIdProductDTO>()
                 .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls))
                 .ReverseMap();

            // WebApi GetByIdProductDTO'dan WebUI UpdateProductDTO'ya
            // Bu, Edit sayfasını doldurmak için gerekli
            CreateMap<DTOsLayer.WebApiDTO.ProductDTOs.GetByIdProductDTO, DTOsLayer.WebUIDTO.ProductDTO.UpdateProductDTO>()
                // API'deki GetByIdProductDTO'nun ImageUrls'ı string listesi,
                // Web UI'daki UpdateProductDTO'nun Images'ı ProductImageDTO listesi.
                // Bu dönüşümü manuel olarak yapıyoruz. API'den gelen URL'ler için Id'yi 0 atıyoruz
                // çünkü API'den ProductImage nesnesinin tam ID'si gelmiyor.
                // Eğer API tarafında GetByIdProductDTO'ya Images koleksiyonunu eklediyseniz,
                // buradan direkt olarak ProductImageDTO'ya eşleyebilirsiniz.
                // Mevcut yapınızdaki API GetByIdProductDTO'da ImageUrls string listesi olduğu için bu şekilde kalır.
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ImageUrls.Select(url => new DTOsLayer.WebUIDTO.ProductDTO.ProductImageDTO { ImageUrl = url, Id = 0 })))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId)); // ProductId'yi de eşle
                                                                                              // Diğer alanlar aynı isimde olduğu için otomatik eşlenecek.

            // WebUI CreateProductDTO'dan WebApi CreateProductDTO'ya
            // ImageFiles burada ignore ediliyor, çünkü controller'da manuel olarak işlenecek.
            CreateMap<DTOsLayer.WebUIDTO.ProductDTO.CreateProductDTO, DTOsLayer.WebApiDTO.ProductDTOs.CreateProductDTO>()
                .ForMember(dest => dest.ImageFiles, opt => opt.Ignore());

            // WebUI UpdateProductDTO'dan WebApi UpdateProductDTO'ya
            // Images burada ignore ediliyor, çünkü controller'da manuel olarak işlenecek.
            CreateMap<DTOsLayer.WebUIDTO.ProductDTO.UpdateProductDTO, DTOsLayer.WebApiDTO.ProductDTOs.UpdateProductDTO>()
                .ForMember(dest => dest.Images, opt => opt.Ignore());



        }
    }
}
