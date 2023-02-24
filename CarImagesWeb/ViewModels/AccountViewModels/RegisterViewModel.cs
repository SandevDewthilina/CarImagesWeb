using System.ComponentModel.DataAnnotations;

namespace CarImagesWeb.ViewModels.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required] [EmailAddress] public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required] public string PhoneNumber { get; set; }

        [Required] public bool AgreedToTerms { get; set; }

        [Required] public string Name { get; set; }
    }
}