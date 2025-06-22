using AutoMapper;
using DTOsLayer.WebUIDTO.BannerDTO;

namespace CarShop.WebUI.Mapping
{
    public class GeneralMapping : Profile
    {
        public GeneralMapping()
        {
            CreateMap<DTOsLayer.WebApiDTO.BannerDTO.ResultBannerDTO, ResultBannerDTO>().ReverseMap();
            CreateMap<DTOsLayer.WebApiDTO.BannerDTO.ResultBannerDTO, UpdateBannerDTO>()
                .ForMember(dest => dest.CarImage, opt => opt.Ignore())
                .ForMember(dest => dest.LogoImage, opt => opt.Ignore())
                .ForMember(dest => dest.ExistingCarImageUrl, opt => opt.MapFrom(src => src.CarImageUrl))
                .ForMember(dest => dest.ExistingLogoImageUrl, opt => opt.MapFrom(src => src.LogoImageUrl));
            CreateMap<CreateBannerDTO, DTOsLayer.WebApiDTO.BannerDTO.CreateBannerDTO>()
                .ForMember(dest => dest.CarImage, opt => opt.Ignore())
                .ForMember(dest => dest.LogoImage, opt => opt.Ignore());
            CreateMap<UpdateBannerDTO, DTOsLayer.WebApiDTO.BannerDTO.UpdateBannerDTO>()
                .ForMember(dest => dest.CarImage, opt => opt.Ignore())
                .ForMember(dest => dest.LogoImage, opt => opt.Ignore())
                .ForMember(dest => dest.ExistingCarImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.ExistingLogoImageUrl, opt => opt.Ignore());

            //Product Mapping

            CreateMap<DTOsLayer.WebApiDTO.ProductDTOs.ProductImageDTO, DTOsLayer.WebUIDTO.ProductDTO.ProductImageDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ReverseMap();

            CreateMap<DTOsLayer.WebApiDTO.ProductDTOs.ResultProductDTO, DTOsLayer.WebUIDTO.ProductDTO.ResultProductDTO>()
                 .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls))
                 .ReverseMap();

            CreateMap<DTOsLayer.WebApiDTO.ProductDTOs.GetByIdProductDTO, DTOsLayer.WebUIDTO.ProductDTO.GetByIdProductDTO>()
                 .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls))
                 .ReverseMap();

            CreateMap<DTOsLayer.WebApiDTO.ProductDTOs.GetByIdProductDTO, DTOsLayer.WebUIDTO.ProductDTO.UpdateProductDTO>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ImageUrls.Select(url => new DTOsLayer.WebUIDTO.ProductDTO.ProductImageDTO { ImageUrl = url, Id = 0 })))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId));
            CreateMap<DTOsLayer.WebUIDTO.ProductDTO.CreateProductDTO, DTOsLayer.WebApiDTO.ProductDTOs.CreateProductDTO>()
                .ForMember(dest => dest.ImageFiles, opt => opt.Ignore());
            CreateMap<DTOsLayer.WebUIDTO.ProductDTO.UpdateProductDTO, DTOsLayer.WebApiDTO.ProductDTOs.UpdateProductDTO>()
                .ForMember(dest => dest.Images, opt => opt.Ignore());



        }
    }
}
