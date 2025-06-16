using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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