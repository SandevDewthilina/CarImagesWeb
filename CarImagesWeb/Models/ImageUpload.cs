using System;

namespace CarImagesWeb.Models
{
    public class ImageUpload
    {
        public int AssetId { get; set; }
        public Asset Asset { get; set; }
        public string UserId { get; set; }
        public string FileName { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}