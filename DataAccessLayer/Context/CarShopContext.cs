using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace DataAccessLayer.Context
{
    public class CarShopContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public CarShopContext(DbContextOptions<CarShopContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                            v => v.ToUniversalTime(),
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(new ValueConverter<DateTime?, DateTime?>(
                            v => v.HasValue ? v.Value.ToUniversalTime() : v,
                            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v));
                    }
                }
            }
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<About> Abouts { get; set; }
        public DbSet<AboutFeature> AboutFeatures { get; set; }
        public DbSet<AboutItem> AboutItems { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<CallBack> CallBacks { get; set; }
        public DbSet<CallBackTitle> BackTitles { get; set; }
        public DbSet<CalltoAction> CalltoActions { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<NewLatest> NewLatests { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Showroom> Showrooms { get; set; }
        public DbSet<WhyUse> WhyUses { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Testimonial> Testimonials { get; set; }
        public DbSet<FeatureSubstance> FeatureSubstances { get; set; }
        public DbSet<WhyUseReason> WhyUseReasons { get; set; }
    }
}