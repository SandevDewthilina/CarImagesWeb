using System;
using System.Collections.Generic;
using CarImagesWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CarImagesWeb.Controllers
{
    public class ImagesController : Controller
    {
        [HttpGet]
        public IActionResult Upload()
        {
            var viewModel = ImageUploadViewModel.DefaultInstance;
            return View(viewModel);
        }
        
        public IActionResult Gallery()
        {
            return View();
        }
    }
}