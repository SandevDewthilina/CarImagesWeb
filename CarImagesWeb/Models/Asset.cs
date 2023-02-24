namespace CarImagesWeb.Models
{
    /// <summary>
    ///     A class that extends Entity and adds a property for asset type.
    /// </summary>
    public class Asset : Entity
    {
        public string Type { get; set; }
    }
}