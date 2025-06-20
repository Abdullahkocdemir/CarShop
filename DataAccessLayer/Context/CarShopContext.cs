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

            builder.Entity<FeatureImage>()
                .HasOne(fi => fi.Feature)
                .WithMany(f => f.FeatureImages)
                .HasForeignKey(fi => fi.FeatureId)
                .OnDelete(DeleteBehavior.Cascade);
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
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<CalltoAction> CalltoActions { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<NewLatest> NewLatests { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Showroom> Showrooms { get; set; }
        public DbSet<WhyUse> WhyUses { get; set; }
        public DbSet<FeatureImage> FeatureImages { get; set; }
        public DbSet<WhyUseReason> WhyUseReasons { get; set; }
    }
}