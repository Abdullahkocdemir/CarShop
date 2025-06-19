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

		}
	}
}
