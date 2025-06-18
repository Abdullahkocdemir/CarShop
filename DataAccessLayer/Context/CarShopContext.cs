using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion; // Bu using'i ekleyin
using System; // DateTime için ekleyin

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

            // Tüm DateTime ve Nullable DateTime tipleri için Value Converter uygula
            // Bu, Entity Framework Core'un DateTime değerlerini veritabanına kaydederken
            // ve okurken nasıl işleyeceğini belirtir.
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        // DateTime için: Kaydederken UTC'ye çevir, okurken Kind'ı UTC olarak işaretle
                        property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                            v => v.ToUniversalTime(),          // Veritabanına yazmadan önce
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc))); // Veritabanından okuduktan sonra
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        // Nullable DateTime için: Null değilse UTC'ye çevir, Kind'ı işaretle
                        property.SetValueConverter(new ValueConverter<DateTime?, DateTime?>(
                            v => v.HasValue ? v.Value.ToUniversalTime() : v, // Veritabanına yazmadan önce
                            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v)); // Veritabanından okuduktan sonra
                    }
                }
            }
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Broadcast> Broadcasts { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<NewLatest> NewLatests { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Showroom> Showrooms { get; set; }
        public DbSet<WhyUse> WhyUses { get; set; }
    }
}