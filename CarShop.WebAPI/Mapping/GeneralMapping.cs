using AutoMapper;
using DTOsLayer.WebApiDTO.BannerDTO;
using DTOsLayer.WebApiDTO.ProductDTOs;
using DTOsLayer.WebApiDTO.BrandDTO;
using EntityLayer.Entities;
using System;
using DTOsLayer.WebApiDTO.ContactDTO;
using DTOsLayer.WebApiDTO.FeatureDTO;
using DTOsLayer.WebApiDTO.NewLatestDTO;
using DTOsLayer.WebApiDTO.ServiceDTO;
using DTOsLayer.WebApiDTO.ShowroomDTO;
using DTOsLayer.WebApiDTO.WhyUseDTO;
using DTOsLayer.WebApiDTO.FeatureImageDTO;
using DTOsLayer.WebApiDTO.WhyUseReasonDTO;
using DTOsLayer.WebApiDTO.AboutFeature;
using DTOsLayer.WebApiDTO.AboutItem;
using DTOsLayer.WebApiDTO.AboutItemDTO;
using DTOsLayer.WebApiDTO.AboutDTO;
using DTOsLayer.WebApiDTO.CallBackDTO;
using DTOsLayer.WebApiDTO.CallBackTitleDTO;
using DTOsLayer.WebApiDTO.CalltoActionDTO;
using DTOsLayer.WebApiDTO.PartnerDTO;
using DTOsLayer.WebApiDTO.StaffDTO;
using DTOsLayer.WebApiDTO.TestimonialDTO;
using DTOsLayer.WebApiDTO.BlogDTO;
using DTOsLayer.WebApiDTO.ModelDTO;
using DTOsLayer.WebApiDTO.ColorDTO;

namespace CarShop.WebAPI.Mapping
{
    public class GeneralMapping : Profile
    {
        public GeneralMapping()
        {
            //Color Mapping

            CreateMap<Color, ResultColorDTO>().ReverseMap();
            CreateMap<Color, GetByIdColorDTO>().ReverseMap();
            CreateMap<Color, CreateColorDTO>().ReverseMap();
            CreateMap<Color, UpdateColorDTO>().ReverseMap();

            //Model Mapping

            CreateMap<Model, ResultModelDTO>().ReverseMap();
            CreateMap<Model, GetByIdModelDTO>().ReverseMap();
            CreateMap<Model, CreateModelDTO>().ReverseMap();
            CreateMap<Model, UpdateModelDTO>().ReverseMap();


            //Blogs Mapping

            CreateMap<Blog, ResultBlogDTO>().ReverseMap();
            CreateMap<Blog, GetByIdBlogDTO>().ReverseMap();
            CreateMap<Blog, CreateBlogDTO>().ReverseMap();
            CreateMap<Blog, UpdateBlogDTO>().ReverseMap();

            CreateMap<CreateBlogDTO, Blog>()
                .ForMember(dest => dest.BannerImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());

            CreateMap<UpdateBlogDTO, Blog>()
                .ForMember(dest => dest.BannerImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());

            //Testimonial Mapping
            CreateMap<CreateTestimonialDTO, Testimonial>().ReverseMap();
            CreateMap<UpdateTestimonialDTO, Testimonial>().ReverseMap();
            CreateMap<ResultTestimonialDTO, Testimonial>().ReverseMap();
            CreateMap<GetByIdTestimonialDTO, Testimonial>().ReverseMap();


            //CalltoAction Mapping

            CreateMap<CalltoAction, ResultCalltoActionDTO>().ReverseMap();
            CreateMap<CalltoAction, GetByIdCalltoActionDTO>().ReverseMap();

            CreateMap<CreateCalltoActionDTO, CalltoAction>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore()).ReverseMap();

            CreateMap<UpdateCalltoActionDTO, CalltoAction>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore()).ReverseMap();


            //CallBackTitle Mapping
            CreateMap<CreateCallBackTitleDTO, CallBackTitle>().ReverseMap();
            CreateMap<GetByIdCallBackTitleDTO, CallBackTitle>().ReverseMap();
            CreateMap<UpdateCallBackTitleDTO, CallBackTitle>().ReverseMap();
            CreateMap<ResultCallBackTitleDTO, CallBackTitle>().ReverseMap();

            //Partner Mapping
            CreateMap<Partner, ResultPartnerDTO>().ReverseMap();
            CreateMap<Partner, GetByIdPartnerDTO>().ReverseMap();

            CreateMap<CreatePartnerDTO, Partner>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());

            CreateMap<UpdatePartnerDTO, Partner>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());

            //Staff Mapping


            CreateMap<Staff, ResultStaffDTO>().ReverseMap();
            CreateMap<Staff, GetByIdStaffDTO>().ReverseMap();

            CreateMap<CreateStaffDTO, Staff>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());

            CreateMap<UpdateStaffDTO, Staff>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());

            //CallBack Mapping
            CreateMap<CreateCallBackDTO, CallBack>().ReverseMap();
            CreateMap<ResultCallBackDTO, CallBack>().ReverseMap();
            CreateMap<UpdateCallBackDTO, CallBack>().ReverseMap();
            CreateMap<UpdateCallBackDTO, CallBack>().ReverseMap();

            //About Mapping
            CreateMap<CreateAboutDTO, About>().ReverseMap();
            CreateMap<ResultAboutDTO, About>().ReverseMap();
            CreateMap<UpdateAboutDTO, About>().ReverseMap();
            CreateMap<GetByIdAboutDTO, About>().ReverseMap();

            //AboutItem Mapping
            CreateMap<CreateAboutItemDTO, AboutItem>().ReverseMap();
            CreateMap<ResultAboutItemDTO, AboutItem>().ReverseMap();
            CreateMap<UpdateAboutItemDTO, AboutItem>().ReverseMap();
            CreateMap<GetByIdAboutItemDTO, AboutItem>().ReverseMap();

            // Product Mappings
            CreateMap<Product, ResultProductDTO>()
                .ForMember(dest => dest.ColorName, opt => opt.MapFrom(src => src.Color != null ? src.Color.Name : string.Empty))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand != null ? src.Brand.BrandName : string.Empty))
                .ForMember(dest => dest.ModelName, opt => opt.MapFrom(src => src.Model != null ? src.Model.Name : string.Empty))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.Images != null ? src.Images.Select(img => img.ImageUrl).ToList() : new List<string>()));

            CreateMap<Product, GetByIdProductDTO>()
                .ForMember(dest => dest.ColorName, opt => opt.MapFrom(src => src.Color != null ? src.Color.Name : string.Empty))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand != null ? src.Brand.BrandName : string.Empty))
                .ForMember(dest => dest.ModelName, opt => opt.MapFrom(src => src.Model != null ? src.Model.Name : string.Empty))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.Images != null ? src.Images.Select(img => img.ImageUrl).ToList() : new List<string>()));
            CreateMap<CreateProductDTO, Product>()
                .ForMember(dest => dest.Images, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore()); 
            CreateMap<UpdateProductDTO, Product>()
                .ForMember(dest => dest.Images, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => DateTime.UtcNow)); 
            CreateMap<ProductImageDTO, ProductImage>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());

            // Banner Mappings)
            CreateMap<Banner, ResultBannerDTO>().ReverseMap();
            CreateMap<CreateBannerDTO, Banner>().ReverseMap();
            CreateMap<UpdateBannerDTO, Banner>().ReverseMap();

            CreateMap<CreateBannerDTO, Banner>()
                .ForMember(dest => dest.CarImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.LogoImageUrl, opt => opt.Ignore());

            CreateMap<UpdateBannerDTO, Banner>()
                .ForMember(dest => dest.CarImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.LogoImageUrl, opt => opt.Ignore());

            CreateMap<Banner, ResultBannerDTO>();

            // Brand Mappings
            CreateMap<CreateBrandDTO, Brand>().ReverseMap();
            CreateMap<UpdateBrandDTO, Brand>().ReverseMap();
            CreateMap<GetByIdBrandDTO, Brand>().ReverseMap();
            CreateMap<ResultBrandDTO, Brand>().ReverseMap();

            //Contact
            CreateMap<CreateContactDTO, Contact>().ReverseMap();
            CreateMap<UpdateContactDTO, Contact>().ReverseMap();
            CreateMap<GetByIdContactDTO, Contact>().ReverseMap();
            CreateMap<ResultContactDTO, Contact>().ReverseMap();


            CreateMap<Feature, ResultFeatureDTO>();
            CreateMap<Feature, GetByIdFeatureDTO>();
            CreateMap<Feature, UpdateFeatureDTO>();

            // Feature Mappings
            CreateMap<Feature, ResultFeatureDTO>()
                .ForMember(dest => dest.FeatureImages, opt => opt.MapFrom(src => src.FeatureImages))
                .ReverseMap();

            CreateMap<Feature, GetByIdFeatureDTO>()
                .ForMember(dest => dest.FeatureImages, opt => opt.MapFrom(src => src.FeatureImages))
                .ReverseMap();
            CreateMap<CreateFeatureDTO, Feature>()
                .ForMember(dest => dest.FeatureImages, opt => opt.Ignore());
            CreateMap<UpdateFeatureDTO, Feature>()
                .ForMember(dest => dest.FeatureImages, opt => opt.Ignore());
            CreateMap<FeatureImage, ResultFeatureImageDTO>().ReverseMap();
            //NewLatest

            CreateMap<CreateNewLatestDTO, NewLatest>().ReverseMap();
            CreateMap<UpdateNewLatestDTO, NewLatest>().ReverseMap();
            CreateMap<GetByIdNewLatestDTO, NewLatest>().ReverseMap();
            CreateMap<ResultNewLatestDTO, NewLatest>().ReverseMap();

            //Service

            CreateMap<Service, ResultServiceDTO>().ReverseMap();
            CreateMap<Service, GetByIdServiceDTO>().ReverseMap();
            CreateMap<CreateServiceDTO, Service>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());
            CreateMap<UpdateServiceDTO, Service>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.ServiceId, opt => opt.Condition(src => src.ServiceId != 0)); 

            //ShowRoom

            CreateMap<CreateShowroomDTO, Showroom>().ReverseMap();
            CreateMap<UpdateShowroomDTO, Showroom>().ReverseMap();
            CreateMap<GetByIdShowroomDTO, Showroom>().ReverseMap();
            CreateMap<ResultShowroomDTO, Showroom>().ReverseMap();

            //WhyUse
            // WhyUse Mappings
            CreateMap<CreateWhyUseDTO, WhyUse>().ReverseMap();
            CreateMap<UpdateWhyUseDTO, WhyUse>().ReverseMap();
            CreateMap<ResultWhyUseDTO, WhyUse>().ReverseMap();
            CreateMap<GetByIdWhyUseDTO, WhyUse>().ReverseMap();

            // WhyUseReason Mappings
            CreateMap<CreateWhyUseReasonDTO, WhyUseReason>().ReverseMap();
            CreateMap<UpdateWhyUseReasonDTO, WhyUseReason>().ReverseMap();
            CreateMap<ResultWhyUseReasonDTO, WhyUseReason>().ReverseMap();


            //AboutFeature Mapping
            CreateMap<AboutFeature, ResultAboutFeatureDTO>().ReverseMap();
            CreateMap<AboutFeature, GetByIdAboutFeatureDTO>().ReverseMap();

            CreateMap<CreateAboutFeatureDTO, AboutFeature>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());

            CreateMap<UpdateAboutFeatureDTO, AboutFeature>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());


        }
    }
}