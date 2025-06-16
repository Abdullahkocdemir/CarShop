using AutoMapper;
using DTOsLayer.WebApiDTO.BannerDTO;
using DTOsLayer.WebApiDTO.ProductDTOs;
using EntityLayer.Entities;

namespace CarShop.WebAPI.Mapping
{
    public class GeneralMapping : Profile
    {
        public GeneralMapping()
        {

            CreateMap<CreateProductDTO, Product>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<UpdateProductDTO, Product>()
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.ProductId, opt => opt.Condition(src => src.ProductId != 0));



            CreateMap<CreateBannerDTO, Banner>().ReverseMap();
            CreateMap<UpdateBannerDTO, Banner>().ReverseMap();
            CreateMap<GetByIdBannerDTO, Banner>().ReverseMap();
            CreateMap<ResultBannerDTO, Banner>().ReverseMap();
        }

    }
}
