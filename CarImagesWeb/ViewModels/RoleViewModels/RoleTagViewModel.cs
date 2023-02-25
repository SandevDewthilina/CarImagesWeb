﻿using System.Collections.Generic;
using CarImagesWeb.Models;

namespace CarImagesWeb.ViewModels.RoleViewModels
{
    public class RoleTagViewModel
    {
        /// <summary>
        /// This property will be initialized in the controller get action method and will be used in the view only.
        /// </summary>
        public Tag Tag { get; set; }
        public bool IsSelected { get; set; }
        public int TagId { get; set; }
    }
}