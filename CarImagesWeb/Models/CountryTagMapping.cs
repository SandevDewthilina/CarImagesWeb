namespace CarImagesWeb.Models
{
    /// <summary>
    ///     A class representing a many-to-many mapping between a country and a tag.
    /// </summary>
    public class CountryTagMapping : TagMapping
    {
        public Country Country { get; set; }
        public int CountryId { get; set; }
    }
}