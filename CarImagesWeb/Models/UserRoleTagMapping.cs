namespace CarImagesWeb.Models
{
    /// <summary>
    ///     A class representing a many-to-many mapping between a user role, a tag, and another entity.
    /// </summary>
    public class UserRoleTagMapping : TagMapping
    {
        public UserRoleTagMapping()
        {
            
        }
        
        public UserRoleTagMapping(Tag tag, UserRole role)
        {
            Tag = Tag;
            UserRole = role;
            TagId = tag.Id;
            UserRoleId = role.Id;
        }

        public UserRole UserRole { get; set; }
        public string UserRoleId { get; set; }
    }
}