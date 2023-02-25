using System.Collections.Generic;

namespace CarImagesWeb.DTOs
{
    public class ImageSearchDto
    {
        //list of tags
        public List<string> Tags { get; set; }

        //list of assets
        public string Asset { get; set; }

        //asset type (vehicle, container)  
        public string AssetType { get; set; }
        public string Country { get; set; }
    }
}