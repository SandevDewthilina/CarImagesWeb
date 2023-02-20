namespace CarImagesWeb.Models
{
    /// <summary>
    /// A class representing a many-to-many mapping between a user role, a tag, and another entity.
    /// </summary>
    public class UserRoleTagMapping : TagMapping
    {
        public UserRole UserRole { get; set; }
        public int UserRoleId { get; set; }
    }
}