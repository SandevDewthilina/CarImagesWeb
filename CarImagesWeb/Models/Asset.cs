using System;

namespace CarImagesWeb.Models
{
    /// <summary>
    ///     A class that extends Entity and adds a property for asset type.
    /// </summary>
    public class Asset : Entity
    {
        public string Type { get; set; }
        public string Stock { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Market { get; set; }
        public string SalesSegment { get; set; }
        public DateTime YardInDate { get; set; }

        public override string ToString()
        {
            return $"{Code} : {Stock}";
        }
    }
}