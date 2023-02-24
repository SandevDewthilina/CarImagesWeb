using System.Collections.Generic;
using CarImagesWeb.Models;

namespace CarImagesWeb.ViewModels.RoleViewModels
{
    public class RoleTagViewModel
    {
        public RoleTagViewModel()
        {
            
        }

        public RoleTagViewModel(Tag tag)
        {
            Tag = tag;
            TagId = tag.Id;
        }

        public Tag Tag { get; set; }
        public bool IsSelected { get; set; }
        public int TagId { get; set; }
    }
}