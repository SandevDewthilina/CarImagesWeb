using Microsoft.AspNetCore.Identity;

namespace CarImagesWeb.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }

    public class UserRole : IdentityRole
    {
        
    }
}