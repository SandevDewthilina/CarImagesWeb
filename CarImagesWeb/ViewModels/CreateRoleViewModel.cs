using System;
using System.ComponentModel.DataAnnotations;

namespace CarImagesWeb.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
        public String RoleName { get; set; }
    }
}
