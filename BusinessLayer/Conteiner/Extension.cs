using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.Abstract;
using DataAccessLayer.EntityFrameWork;
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


            Services.AddScoped<IFeatureImageDal, EfFeatureImageDal>();
            Services.AddScoped<IFeatureImageService, FeatureImageManager>();


            Services.AddScoped<IWhyUseReasonDal, EfWhyUseReasonDal>();
            Services.AddScoped<IWhyUseReasonService, WhyUseReasonManager>();


            Services.AddScoped<ICalltoActionDal, EfCalltoActionDal>();
            Services.AddScoped<ICalltoActionService, CalltoActionManager>();


        }
    }
}
