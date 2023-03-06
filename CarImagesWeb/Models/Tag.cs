namespace CarImagesWeb.Models
{
    /// <summary>
    ///     A class for Tags that extends Entity and without any additional properties yet.
    /// </summary>
    public class Tag : Entity
    {
        public int CountryId { get; set; }
        public string Type { get; set; }

        public Country Country { get; set; }
    }
}