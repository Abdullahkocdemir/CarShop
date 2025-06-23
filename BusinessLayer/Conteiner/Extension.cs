using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.Abstract;
using DataAccessLayer.EntityFrameWork;
using EntityLayer.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Conteiner
{
    public static class Extension
    {
        public static void ConteinerDependencies(this IServiceCollection Services)
        {
            Services.AddScoped<IBannerDal, EfBannerDal>();
            Services.AddScoped<IBannerService, BannerManager>();

            Services.AddScoped<IBrandDal, EfBrandDal>();
            Services.AddScoped<IBrandService, BrandManager>();

            Services.AddScoped<IContactDal, EfContactDal>();
            Services.AddScoped<IContactService, ContactManager>();

            Services.AddScoped<IFeatureDal, EfFeatureDal>();
            Services.AddScoped<IFeatureService, FeatureManager>();

            Services.AddScoped<INewLatestDal, EfNewLatestDal>();
            Services.AddScoped<INewLatestService, NewLatestManager>();

            Services.AddScoped<IProductDal, EfProductDal>();
            Services.AddScoped<IProductService, ProductManager>();

            Services.AddScoped<IServiceDal, EfServiceDal>();
            Services.AddScoped<IServiceService, ServiceManager>();


            Services.AddScoped<IShowroomDal, EfShowroomDal>();
            Services.AddScoped<IShowroomService, ShowroomManager>();


            Services.AddScoped<IWhyUseDal, EfWhyUseDal>();
            Services.AddScoped<IWhyUseService, WhyUseManager>();

            Services.AddScoped<IWhyUseReasonDal, EfWhyUseReasonDal>();
            Services.AddScoped<IWhyUseReasonService, WhyUseReasonManager>();


            Services.AddScoped<ICalltoActionDal, EfCalltoActionDal>();
            Services.AddScoped<ICalltoActionService, CalltoActionManager>();

            Services.AddScoped<IAboutDal, EfAboutDal>();
            Services.AddScoped<IAboutService, AboutManager>();


            Services.AddScoped<IAboutFeatureDal, EfAboutFeatureDal>();
            Services.AddScoped<IAboutFeatureService, AboutFeatureManager>();



            Services.AddScoped<IAboutItemDal, EfAboutItemDal>();
            Services.AddScoped<IAboutItemService, AboutItemManager>();


            Services.AddScoped<IBlogDal, EfBlogDal>();
            Services.AddScoped<IBlogService, BlogManager>();


            Services.AddScoped<ICallBackDal, EfCallBackDal>();
            Services.AddScoped<ICallBackService, CallBackManager>();

            Services.AddScoped<ICallBackTitleDal, EfCallBackTitleDal>();
            Services.AddScoped<ICallBackTitleService, CallBackTitleManager>();

            Services.AddScoped<IPartnerDal, EfPartnerDal>();
            Services.AddScoped<IPartnerService, PartnerManager>();


            Services.AddScoped<IStaffDal, EfStaffDal>();
            Services.AddScoped<IStaffService, StaffManager>();


            Services.AddScoped<ITestimonialDal, EfTestimonialDal>();
            Services.AddScoped<ITestimonialService, TestimonialManager>();


            Services.AddScoped<IModelDal, EfModelDal>();
            Services.AddScoped<IModelService, ModelManager>();


            Services.AddScoped<IColorDal, EfColorDal>();
            Services.AddScoped<IColorService, ColorManager>();

            Services.AddScoped<IFeatureSubstanceDal, EfFeatureSubstanceDal>();
            Services.AddScoped<IFeatureSubstanceService, FeatureSubstanceManager>();



        }
    }
}
