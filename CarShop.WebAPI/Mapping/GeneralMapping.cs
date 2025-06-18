using AutoMapper;
using DTOsLayer.WebApiDTO.BannerDTO;
using DTOsLayer.WebApiDTO.ProductDTOs;
using DTOsLayer.WebApiDTO.BrandDTO; // Brand DTO'larını ekledik
using EntityLayer.Entities;
using System;
using DTOsLayer.WebApiDTO.BroadcastDTO;
using DTOsLayer.WebApiDTO.ContactDTO;
using DTOsLayer.WebApiDTO.FeatureDTO;
using DTOsLayer.WebApiDTO.NewLatestDTO;
using DTOsLayer.WebApiDTO.ServiceDTO;
using DTOsLayer.WebApiDTO.ShowroomDTO;
using DTOsLayer.WebApiDTO.WhyUseDTO;

namespace CarShop.WebAPI.Mapping
{
    public class GeneralMapping : Profile
    {
        public GeneralMapping()
        {
            // Product Mappings
            CreateMap<CreateProductDTO, Product>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<UpdateProductDTO, Product>()
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.ProductId, opt => opt.Condition(src => src.ProductId != 0));

            // Banner Mappings
            CreateMap<CreateBannerDTO, Banner>().ReverseMap();
            CreateMap<UpdateBannerDTO, Banner>().ReverseMap();
            CreateMap<GetByIdBannerDTO, Banner>().ReverseMap();
            CreateMap<ResultBannerDTO, Banner>().ReverseMap();

            // Brand Mappings
            CreateMap<CreateBrandDTO, Brand>().ReverseMap();
            CreateMap<UpdateBrandDTO, Brand>().ReverseMap();
            CreateMap<GetByIdBrandDTO, Brand>().ReverseMap();
            CreateMap<ResultBrandDTO, Brand>().ReverseMap();


            //BroadCast
            CreateMap<CreateBroadcastDTO, Broadcast>().ReverseMap();
            CreateMap<UpdateBroadcastDTO, Broadcast>().ReverseMap();
            CreateMap<GetByIdBroadcastDTO, Broadcast>().ReverseMap();
            CreateMap<ResultBroadcastDTO, Broadcast>().ReverseMap();


            //Contact
            CreateMap<CreateContactDTO, Contact>().ReverseMap();
            CreateMap<UpdateContactDTO, Contact>().ReverseMap();
            CreateMap<GetByIdContactDTO, Contact>().ReverseMap();
            CreateMap<ResultContactDTO, Contact>().ReverseMap();


            // Entity'den WebApi DTO'larına
            CreateMap<Feature, ResultFeatureDTO>();
            CreateMap<Feature, GetByIdFeatureDTO>();
            CreateMap<Feature, UpdateFeatureDTO>(); // UI'a mevcut veriyi doldurmak için kullanılabilir

            // WebApi DTO'larından Entity'ye
            CreateMap<CreateFeatureDTO, Feature>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore()).ReverseMap(); // ImageUrl'i manuel yöneteceğimiz için yoksay
            CreateMap<UpdateFeatureDTO, Feature>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore()).ReverseMap() ; // ImageUrl'i manuel yöneteceğimiz için yoksay
            CreateMap<GetByIdFeatureDTO, Feature>().ReverseMap();


            //NewLatest

            CreateMap<CreateNewLatestDTO, NewLatest>().ReverseMap();
            CreateMap<UpdateNewLatestDTO, NewLatest>().ReverseMap();
            CreateMap<GetByIdNewLatestDTO, NewLatest>().ReverseMap();
            CreateMap<ResultNewLatestDTO, NewLatest>().ReverseMap();

            //Service

            CreateMap<CreateServiceDTO, Service>().ReverseMap();
            CreateMap<UpdateServiceDTO, Service>().ReverseMap();
            CreateMap<GetByIdServiceDTO, Service>().ReverseMap();
            CreateMap<ResultServiceDTO, Service>().ReverseMap();

            //ShowRoom

            CreateMap<CreateShowroomDTO, Showroom>().ReverseMap();
            CreateMap<UpdateShowroomDTO, Showroom>().ReverseMap();
            CreateMap<GetByIdShowroomDTO, Showroom>().ReverseMap();
            CreateMap<ResultShowroomDTO, Showroom>().ReverseMap();

            //WhyUse
            CreateMap<CreateWhyUseDTO, WhyUse>().ReverseMap();
            CreateMap<UpdateWhyUseDTO, WhyUse>().ReverseMap();
            CreateMap<GetByIdWhyUseDTO, WhyUse>().ReverseMap();
            CreateMap<ResultWhyUseDTO, WhyUse>().ReverseMap();


        }
    }
}