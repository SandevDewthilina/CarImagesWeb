using System.ComponentModel.DataAnnotations;

namespace CarImagesWeb.ViewModels
{
    public class CreateEntityViewModel
    {
        [Required] public string Code { get; set; }

        [Required] public string Name { get; set; }
    }
}