using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarImagesWeb.ViewModels
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            Users = new List<string>();
        }

        public String Id { get; set; }

        [Required(ErrorMessage = "Role name is required")]
        public String RoleName { get; set; }

        public List<String> Users { get; set; }
    }
}
