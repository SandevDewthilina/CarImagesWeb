using System;

namespace CarImagesWeb.Models
{
    public class ImageUpload : TagMapping
    {
        public int AssetId { get; set; }
        public Asset Asset { get; set; }
        public string UserId { get; set; }
        public string FileName { get; set; }
    }
}