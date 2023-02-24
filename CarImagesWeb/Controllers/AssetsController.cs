using System;
using System.Threading.Tasks;
using CarImagesWeb.Services;
using CarImagesWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CarImagesWeb.Controllers
{
    public class AssetsController : Controller
    {
        private readonly IAssetsHandler _assetsHandler;

        public AssetsController(IAssetsHandler assetsHandler)
        {
            _assetsHandler = assetsHandler;
        }
        
        public IActionResult Manage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Manage(UpdateAssetsViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.IsReset)
            {
                if (model.File == null)
                {
                    ModelState.AddModelError("Reset", "Please upload a file.");
                    return View(model);
                }

                var fileInput = model.File;

                void ErrorHandleCallback(Exception e)
                {
                    // Handle the exception
                    ModelState.AddModelError("Reset", e.Message);
                }

                await _assetsHandler.ResetAssetsAsync(fileInput, ErrorHandleCallback);
            }
            else
            {
                if (model.DeleteFile == null)
                {
                    ModelState.AddModelError("Delete", "Please upload a file.");
                    return View(model);
                }

                var fileInput = model.DeleteFile;

                void ErrorHandleCallback(Exception e)
                {
                    // Handle the exception
                    ModelState.AddModelError("Delete", e.Message);
                }

                await _assetsHandler.DeleteAssetsAsync(fileInput, ErrorHandleCallback);
            }

            if (ModelState.ErrorCount > 0) return View(model);

            // Return a success response
            return RedirectToAction("List", "Assets");
        }
        
        public IActionResult List()
        {
            throw new NotImplementedException();
        }
    }

    public class AssetRecord
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
    }
}