using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CarImagesWeb.DbOperations;
using CarImagesWeb.Models;
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
        private readonly IAssetRepository _assetRepository;

        public AssetsController(IAssetsHandler assetsHandler, ICsvHandler csvHandler, IAssetRepository assetRepository)
        {
            _assetsHandler = assetsHandler;
            _csvHandler = csvHandler;
            _assetRepository = assetRepository;
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
            else if (model.IsAssignment)
            {
                if (model.ContainerVehicleAssignFile == null)
                {
                    ModelState.AddModelError("Assign", "Please upload a file.");
                    return View(model);
                }

                var fileInput = model.ContainerVehicleAssignFile;

                void ErrorHandleCallback(Exception e)
                {
                    // Handle the exception
                    ModelState.AddModelError("Assign", e.Message);
                }
                
                await _assetsHandler.AssignAssets(fileInput, ErrorHandleCallback);
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

                try
                {
                    await _assetsHandler.DeleteAssetsAsync(fileInput, ErrorHandleCallback);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("Delete", e.Message);
                }
            }

            if (ModelState.ErrorCount > 0) return View(model);
            // Return a success response
            ViewBag.status = true;
            return View(model);
        }

        public async Task<IActionResult> List()
        {
            // Get the assets from the database
            // var assets = await _assetsHandler.GetAllAssets();
            ViewBag.vehicleContainerMappings = await _assetsHandler.GetAllContainerVehicleMappings();
            return View(new ListSearchModel());
        }

        [HttpPost]
        public async Task<IActionResult> List(ListSearchModel model)
        {
            ViewBag.vehicleContainerMappings = await _assetsHandler.GetAllContainerVehicleMappings();
            if (string.IsNullOrWhiteSpace(model.Code) || string.IsNullOrEmpty(model.Code))
            {
                model.Assets = await _assetRepository
                    .GetAllAsync(
                        a => a.PurchaseDate.Date >= model.StartDate.Date && a.PurchaseDate.Date <= model.EndDate.Date
                    );
            }
            else
            {
                model.Assets = await _assetRepository
                    .GetAllAsync(
                        a => a.Code.Equals(model.Code)
                    );
            }
            return View(model);
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
        public string Stock { get; set; }
        public string PurchaseDate { get; set; }
        public string Market { get; set; }
        public string SalesSegment { get; set; }

        public bool IsNonEmpty()
        {
            return !string.IsNullOrEmpty(Name)
                   && !string.IsNullOrEmpty(Code)
                   && !string.IsNullOrEmpty(Type)
                   && !string.IsNullOrEmpty(Stock)
                   && !string.IsNullOrEmpty(PurchaseDate)
                   && !string.IsNullOrEmpty(Market)
                   && !string.IsNullOrEmpty(SalesSegment);


        }
    }

    public class ListSearchModel
    {
        public ListSearchModel()
        {
            StartDate = DateTime.Now;
            EndDate = DateTime.Now;
            Assets = new List<Asset>();
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Code { get; set; }
        public List<Asset> Assets { get; set; }
    }

    public class AssignmentRecord
    {
        public string ContainerCode { get; set; }
        public string VehicleCode { get; set; }
    }
}