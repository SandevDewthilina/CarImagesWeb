using System.Collections.Generic;
using CarImagesWeb.DTOs;

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
            },
            CountryCodes = new List<string>()
            {
                "Country001", "Country002", "Country003"
            }
        };

        public IEnumerable<string> Vehicles { get; set; }
        public IEnumerable<string> Containers { get; set; }
        public IEnumerable<string> VehicleTags { get; set; }
        public IEnumerable<string> ContainerTags { get; set; }
        public IEnumerable<string> CountryCodes { get; set; }
        public ImageUploadDto ImageUploadDto { get; }
    }
}