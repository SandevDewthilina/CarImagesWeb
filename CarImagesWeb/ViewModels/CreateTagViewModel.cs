using System.ComponentModel.DataAnnotations;

namespace CarImagesWeb.ViewModels
{
    public class CreateTagViewModel
    {
        [Required] public string Code { get; set; }

        [Required] public string Name { get; set; }
    }
}