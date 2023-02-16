using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CarImagesWeb.ViewModels
{
    public class ImageUploadViewModel
    {

        public ImageUploadViewModel()
        {
            
        }

        //default singleton instance
        public static ImageUploadViewModel DefaultInstance { get; } = new ImageUploadViewModel()
        {
            VehicleTags = new List<string>()
            {
                "Front", "Rear", "Side"
            },
            Vehicles = new List<string>(){
                "Vehicle/0000", "Vehicle/1111", "Vehicle/2222"
            },
            ContainerTags = new List<string>()
            {
                "Front", "Rear", "Side"
            },
            Containers = new List<string>(){
                "Container/0000", "Container/1111", "Container/2222"
            }
        };
        

        public string ImageCategory { get; set; }
        public IEnumerable<string> Vehicles { get; set; }
        public string Vehicle { get; set; }
        public IEnumerable<string> Containers { get; set; }
        public string Container { get; set; }
        public string Tag { get; set; }
        public IEnumerable<string> VehicleTags { get; set; }
        public IEnumerable<string> ContainerTags { get; set; }
        public IFormFile[] Files { get; set; }
    }
}