using CarImagesWeb.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarImagesWeb.DbContext
{
    public class CarImagesDbContext : IdentityDbContext<ApplicationUser>
    {
        public CarImagesDbContext(DbContextOptions<CarImagesDbContext> options) : base(options)
        {
            
        }

        public DbSet<Asset> Assets { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<CountryTagMapping> CountryTagMappings { get; set; }
        public DbSet<UserRoleTagMapping> UserRoleTagMappings { get; set; }
        public DbSet<ImageUpload> ImageUploads { get; set; }

    }
}