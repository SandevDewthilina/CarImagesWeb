using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarImagesWeb.DbOperations;
using CarImagesWeb.Models;
using CarImagesWeb.Services;
using CarImagesWeb.ViewModels;
using CsvHelper;
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
        private readonly IImagesHandler _imagesHandler;
        private readonly IImagesRepository _imagesRepository;
        private readonly ITagRepository _tagRepository;

        public AssetsController(IAssetsHandler assetsHandler, ICsvHandler csvHandler, 
            IAssetRepository assetRepository, IImagesHandler imagesHandler,
            IImagesRepository imagesRepository, ITagRepository tagRepository)
        {
            _assetsHandler = assetsHandler;
            _csvHandler = csvHandler;
            _assetRepository = assetRepository;
            _imagesHandler = imagesHandler;
            _imagesRepository = imagesRepository;
            _tagRepository = tagRepository;
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
            return View("SuccessPage");
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
                        a => a.YardInDate.Date >= model.StartDate.Date && a.YardInDate.Date <= model.EndDate.Date
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
        
        public async Task<IActionResult> ExportDataForUploadCount()
        {
            // Create a new MemoryStream to hold the CSV data
            var memoryStream = new MemoryStream();

            // Create a new StreamWriter using the memory stream and UTF-8 encoding
            var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);

            // Create a new CsvWriter using the StreamWriter
            var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

            // Write the data to the CSV file
            var dictionary= new Dictionary<string, Dictionary<string, int>>();
            var tagList = await _tagRepository.GetAllAsync();
            var assetList = await _assetRepository.GetAllAsync();
            var imageUploads = await _imagesRepository.GetAllAsync();

            

            var imageUploadDictionary = new Dictionary<string, int>();
            foreach (var imageUpload in imageUploads)
            {
                var key = imageUpload.Asset.Code + "#" + imageUpload.Tag.Code;
                if (imageUploadDictionary.ContainsKey(key))
                {
                    // var tempValue = imageUploadDictionary[key];
                    // imageUploadDictionary.Remove(key);
                    // imageUploadDictionary.Add(key, tempValue + 1);
                    imageUploadDictionary[key] += 1;
                }
                else
                {
                    imageUploadDictionary.Add(key, 1);
                }
            }
            
            foreach (var asset in assetList)
            {
                var tempDic = tagList.ToDictionary(tag => tag.Code, tag => imageUploadDictionary.ContainsKey(asset.Code + "#" + tag.Code) ? imageUploadDictionary[asset.Code + "#" + tag.Code] : 0);
                dictionary.Add(asset.Code,tempDic);
            }
            
            csvWriter.WriteField("");
            // var outStirng = " ";
            foreach (var tag in tagList)
            {
                csvWriter.WriteField(tag.Code);
                // outStirng += "," + tag.Code;
            }
            await csvWriter.NextRecordAsync();
            // outStirng += "\n";
            
            foreach (var row in dictionary)
            {
                csvWriter.WriteField(row.Key);
                // outStirng += row.Key;
                foreach (var cell in row.Value)
                {
                    csvWriter.WriteField(cell.Value);
                    // outStirng += "," + cell.Value;
                }

                await csvWriter.NextRecordAsync();
                // outStirng += "\n";
            }
            
            
            // Flush the CsvWriter and StreamWriter
            await csvWriter.FlushAsync();
            await streamWriter.FlushAsync();

            // Reset the position of the MemoryStream to the beginning
            memoryStream.Position = 0;

            // Return the CSV file as a FileStreamResult
            return File(memoryStream, "text/csv", "data.csv");
            // return Json(outStirng);
        }

        public async Task<IActionResult> DeleteAssetFromId(int Id)
        {
            try
            {
                await _assetsHandler.DeleteAssetRecordFromId(Id);
                return Json(new {success = true});
            }
            catch (Exception e)
            {
                return Json(new {success = false, error = e.Message});
            }
        }

        public async Task<IActionResult> EditAsset(int Id)
        {
            return View(await _assetRepository.GetAsync(a => a.Id == Id));
        }
        
        [HttpPost]
        public async Task<IActionResult> EditAsset(Asset asset)
        {
            await _assetRepository.UpdateAsync(asset);
            return View("SuccessPage");
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