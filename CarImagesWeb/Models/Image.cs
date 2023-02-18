using System;

namespace CarImagesWeb.Models
{
    public class Image
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Url { get; set; }
    }
}