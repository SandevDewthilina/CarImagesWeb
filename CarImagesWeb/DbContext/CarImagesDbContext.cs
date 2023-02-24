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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CountryTagMapping>()
                .HasKey(c => new {c.CountryId, c.TagId});

            modelBuilder.Entity<UserRoleTagMapping>()
                .HasKey(c => new {c.UserRoleId, c.TagId});
        }

        //get the dbset for the entity
        public DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class
        {
            return Set<TEntity>();
        }
    }
}