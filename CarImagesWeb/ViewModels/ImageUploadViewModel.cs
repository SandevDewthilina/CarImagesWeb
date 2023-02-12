using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CarImagesWeb.ViewModels
{
    public class ImageUploadViewModel
    {
        public ImageUploadViewModel(IEnumerable<string> tags, IEnumerable<string> vehicleTypes)
        {
            Tags = tags;
            VehicleTypes = vehicleTypes;
        }

        public ImageUploadViewModel()
        {
            
        }

        //default singleton instance
        public static ImageUploadViewModel DefaultInstance { get; } = new ImageUploadViewModel(
            new List<string>()
            {
                "Front", "Rear", "Side"
            },
            new List<string>()
            {
                "Car", "Truck", "SUV"
            });

        public string ContainerId { get; set; }
        public string ImageCategory { get; set; }
        public IEnumerable<string> VehicleTypes { get; }
        public string VehicleType { get; set; }
        public string VehicleNumber { get; set; }
        public string ChassisNumber { get; set; }
        public string Tag { get; set; }
        public IEnumerable<string> Tags { get; }
        public IFormFile[] Files { get; set; }
    }
}