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
        
        [HttpPost]
        public IActionResult Upload(ImageUploadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            //Get image from request
            var image = Request.Form.Files?[0];
            //Get image name
            var imageName = image?.FileName;
            //print out the image name
            Console.WriteLine($"Image name: {imageName}");
            
            //print out the model properties using system reflection
            var properties = model.GetType().GetProperties();
            foreach (var property in properties)
            {
                Console.WriteLine($"{property.Name}: {property.GetValue(model)}");
            }
            
            return View("Upload", ImageUploadViewModel.DefaultInstance);
        }
        
        public IActionResult Gallery()
        {
            return View();
        }
    }
}