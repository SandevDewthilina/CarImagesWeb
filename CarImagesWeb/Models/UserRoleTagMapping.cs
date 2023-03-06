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
        
        public UserRoleTagMapping(Tag tag, UserRole role, bool allowUpload, bool allowDownload)
        {
            TagId = tag.Id;
            UserRoleId = role.Id;
            AllowDownload = allowDownload;
            AllowUpload = allowUpload;
        }

        public UserRole UserRole { get; set; }
        public string UserRoleId { get; set; }
        public bool AllowUpload { get; set; }
        public bool AllowDownload { get; set; }
    }
}