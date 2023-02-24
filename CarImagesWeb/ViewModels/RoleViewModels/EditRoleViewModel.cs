using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CarImagesWeb.Models;

namespace CarImagesWeb.ViewModels.RoleViewModels
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            Users = new List<string>();
            Tags = new List<Tag>();
        }

        public string Id { get; set; }

        [Required(ErrorMessage = "Role name is required")]
        public string RoleName { get; set; }

        public List<string> Users { get; set; }
        
        public List<Tag> Tags { get; set; }
    }
}