using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarImagesWeb.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }

    public class UserRole 
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
