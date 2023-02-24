using System;
using System.IO;
using System.Threading.Tasks;
using CarImagesWeb.Services;
using CarImagesWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarImagesWeb.Controllers
{
    [Authorize]
    public class AssetsController : Controller
    {
        private readonly IAssetsHandler _assetsHandler;
        private readonly ICsvHandler _csvHandler;

        public AssetsController(IAssetsHandler assetsHandler, ICsvHandler csvHandler)
        {
            _assetsHandler = assetsHandler;
            _csvHandler = csvHandler;
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
        
        public async Task<IActionResult> List()
        {
            // Get the assets from the database
            var assets = await _assetsHandler.GetAllAssets();
            return View(assets);
        }
        
        [Obsolete("This method is incomplete and is intended for testing purposes only.")]
        public async Task<FileStreamResult> Export()
        {
            // Get the assets from the database
            var assets = await _assetsHandler.GetAllAssets();
            // assets to asset records
            var assetRecords = assets.ConvertAll(a => new AssetRecord
            {
                Name = a.Name,
                Code = a.Code,
                Type = a.Type
            });
            // asset records to csv
            
            var memoryStream = new MemoryStream(await _csvHandler.WriteCsvAsync(assetRecords));
            
            var file = new FormFile(memoryStream, 0, memoryStream.Length, null, "data.csv");
            
            return new FileStreamResult(file.OpenReadStream(), "text/csv")
            {
                FileDownloadName = "export.csv"
            };
        }
    }

    public class AssetRecord
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
    }
}