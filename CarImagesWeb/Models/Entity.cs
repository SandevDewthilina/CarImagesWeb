namespace CarImagesWeb.Models
{
    /// <summary>
    ///     An abstract base class representing an entity with an ID, name, and code.
    /// </summary>
    public abstract class Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public override string ToString()
        {
            return $"{Code}:{Name}";
        }
    }
}