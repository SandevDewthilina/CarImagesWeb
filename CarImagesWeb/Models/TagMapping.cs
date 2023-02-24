namespace CarImagesWeb.Models
{
    /// <summary>
    ///     A abstract base class used to represent a many-to-many mapping between a tag and another entity.
    /// </summary>
    public abstract class TagMapping
    {
        public Tag Tag { get; set; }
        public int TagId { get; set; }
    }
}