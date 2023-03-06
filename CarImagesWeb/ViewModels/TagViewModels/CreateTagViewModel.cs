using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CarImagesWeb.Models;

namespace CarImagesWeb.ViewModels.TagViewModels
{
    public class CreateTagViewModel : CreateEntityViewModel
    {
        [Required]
        public int SelectedCountryId { get; set; }
        [Required] public string Type { get; set; }

        public List<Country> Countries { get; set; }
    }
}