using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CarImagesWeb.Models;

namespace CarImagesWeb.ViewModels
{
    public class CreateEntityViewModel
    {
        [Required] public string Code { get; set; }

        [Required] public string Name { get; set; }
        
    }
}