using System.Collections.Generic;
using CarImagesWeb.DTOs;
using CarImagesWeb.Models;

namespace CarImagesWeb.ViewModels
{
    public class ImageUploadViewModel
    {
        public IEnumerable<Asset> Vehicles { get; set; }
        public IEnumerable<Asset> Containers { get; set; }
        public IEnumerable<Tag> VehicleTags { get; set; }
        public IEnumerable<Tag> ContainerTags { get; set; }
        public IEnumerable<string> CountryCodes { get; set; }
        public ImageUploadDto ImageUploadDto { get; }
    }
}