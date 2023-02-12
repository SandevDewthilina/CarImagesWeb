using System;
using System.ComponentModel.DataAnnotations;

namespace CarImagesWeb.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public String Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public String Password { get; set; }
        
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
