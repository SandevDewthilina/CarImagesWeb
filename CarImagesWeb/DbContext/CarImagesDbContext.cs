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
    }
}