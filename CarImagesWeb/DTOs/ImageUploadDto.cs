using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarImagesWeb.DTOs
{
    public class ImageUploadDto
    {
        public string ImageCategory { get; set; }
        public string CountryCode { get; set; }
        public string VehicleId { get; set; }
        public string ContainerId { get; set; }
        public string VehicleTagId { get; set; }
        public string ContainerTagId { get; set; }
        public IFormFile File { get; set; }
        public IFormFileCollection Files { get; set; }
    }
}