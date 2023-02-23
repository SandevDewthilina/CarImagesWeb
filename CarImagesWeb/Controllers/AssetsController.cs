using System.Globalization;
using System.IO;
using System.Linq;
using CarImagesWeb.Models;
using CarImagesWeb.ViewModels;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace CarImagesWeb.Controllers
{
    public class AssetsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
       
        [HttpPost]
        public IActionResult Index(UpdateAssetsViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            
            var fileInput = model.File;

            // Get the file extension
            var fileExtension = Path.GetExtension(fileInput.FileName);

            switch (fileExtension)
            {
                case ".xlsx":
                {
                    using var package = new ExcelPackage(fileInput.OpenReadStream());
                    // Get the first worksheet
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    // Process the data in the worksheet
                    break;
                }
                case ".csv":
                {
                    using var reader = new StreamReader(fileInput.OpenReadStream());
                    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                    // Read the CSV data into a list of Asset objects
                    var assets = csv.GetRecords<AssetRecord>().ToList();
                    // Process the data in the list
                    break;
                }
            }

            // Return a success response
            return Ok();
        }

    }

    public class AssetRecord
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
    }
}