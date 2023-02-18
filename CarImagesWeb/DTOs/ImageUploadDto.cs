using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarImagesWeb.DTOs
{
    public class ImageUploadDto
    {
        public string ImageCategory { get; set; }
        public string CountryCode { get; set; }
        public string Vehicle { get; set; }
        public string Container { get; set; }
        public string VehicleTag { get; set; }
        public string ContainerTag { get; set; }
    }
}