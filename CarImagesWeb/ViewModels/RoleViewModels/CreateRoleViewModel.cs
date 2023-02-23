using System.ComponentModel.DataAnnotations;

namespace CarImagesWeb.ViewModels.RoleViewModels
{
    public class CreateRoleViewModel
    {
        [Required] public string RoleName { get; set; }
    }
}