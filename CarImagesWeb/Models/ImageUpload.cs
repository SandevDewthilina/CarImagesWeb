namespace CarImagesWeb.Models
{
    public class ImageUpload
    {
        /// <summary>
        ///     Primary key for the ImageUpload table.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Foreign key for the Asset table.
        /// </summary>
        public int AssetId { get; set; }

        /// <summary>
        ///     Navigation property for the Asset table.
        /// </summary>
        public Asset Asset { get; set; }

        /// <summary>
        ///     Foreign key for the user table.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        ///     URL for the image.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        ///     Foreign key for the Tag table.
        /// </summary>
        public int TagId { get; set; }

        /// <summary>
        ///     Navigation property for the Tag table.
        /// </summary>
        public Tag Tag { get; set; }

        /// <summary>
        ///     Foreign key for the Country table.
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        ///     Navigation property for the Country table.
        /// </summary>
        public Country Country { get; set; }
    }
}