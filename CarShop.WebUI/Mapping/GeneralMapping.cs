using AutoMapper;
using DTOsLayer.WebUIDTO.BannerDTO;
using DTOsLayer.WebUIDTO.WhyUseDTO;
using DTOsLayer.WebUIDTO.WhyUseItemDTO;
using EntityLayer.Entities;


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
            CreateMap<DTOsLayer.WebApiDTO.ProductDTOs.ResultProductDTO, DTOsLayer.WebUIDTO.ProductDTO.ResultProductDTO>().ReverseMap();
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








            CreateMap<ResultWhyUseDTO, WhyUse>().ReverseMap();
            CreateMap<CreateWhyUseDTO, WhyUse>().ReverseMap();
            CreateMap<UpdateWhyUseDTO, WhyUse>().ReverseMap();
            CreateMap<GetByIdWhyUseDTO, WhyUse>().ReverseMap();


            CreateMap<ResultWhyUseItemDTO, WhyUseItem>().ReverseMap();
            CreateMap<CreateWhyUseItemDTO, WhyUseItem>().ReverseMap();
            CreateMap<UpdateWhyUseItemDTO, WhyUseItem>().ReverseMap();







            // Product mappings
            CreateMap<DTOsLayer.WebApiDTO.ProductDTOs.ResultProductDTO, DTOsLayer.WebUIDTO.ProductDTO.ResultProductDTO>()
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls));
            CreateMap<DTOsLayer.WebUIDTO.ProductDTO.ResultProductDTO, DTOsLayer.WebApiDTO.ProductDTOs.ResultProductDTO>();

            // Brand mappings
            CreateMap<EntityLayer.Entities.Brand, DTOsLayer.WebApiDTO.BrandDTO.ResultBrandDTO>();
            CreateMap<DTOsLayer.WebApiDTO.BrandDTO.CreateBrandDTO, EntityLayer.Entities.Brand>();
            CreateMap<DTOsLayer.WebApiDTO.BrandDTO.UpdateBrandDTO, EntityLayer.Entities.Brand>();
            CreateMap<EntityLayer.Entities.Brand, DTOsLayer.WebApiDTO.BrandDTO.GetByIdBrandDTO>();
            CreateMap<DTOsLayer.WebApiDTO.BrandDTO.ResultBrandDTO, DTOsLayer.WebUIDTO.BrandDTO.ResultBrandDTO>(); 

            CreateMap<EntityLayer.Entities.Model, DTOsLayer.WebApiDTO.ModelDTO.ResultModelDTO>();
            CreateMap<DTOsLayer.WebApiDTO.ModelDTO.CreateModelDTO, EntityLayer.Entities.Model>();
            CreateMap<DTOsLayer.WebApiDTO.ModelDTO.UpdateModelDTO, EntityLayer.Entities.Model>();
            CreateMap<EntityLayer.Entities.Model, DTOsLayer.WebApiDTO.ModelDTO.GetByIdModelDTO>();
            CreateMap<DTOsLayer.WebApiDTO.ModelDTO.ResultModelDTO, DTOsLayer.WebUIDTO.ModelsDTO.ResultModelDTO>(); 

            CreateMap<EntityLayer.Entities.Color, DTOsLayer.WebApiDTO.ColorDTO.ResultColorDTO>();
            CreateMap<DTOsLayer.WebApiDTO.ColorDTO.CreateColorDTO, EntityLayer.Entities.Color>();
            CreateMap<DTOsLayer.WebApiDTO.ColorDTO.UpdateColorDTO, EntityLayer.Entities.Color>();
            CreateMap<EntityLayer.Entities.Color, DTOsLayer.WebApiDTO.ColorDTO.GetByIdColorDTO>();
            CreateMap<DTOsLayer.WebApiDTO.ColorDTO.ResultColorDTO, DTOsLayer.WebUIDTO.ColorDTO.ResultColorDTO>(); 

        }
    }
}
