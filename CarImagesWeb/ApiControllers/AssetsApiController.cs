using System.Collections.Generic;
using System.Threading.Tasks;
using CarImagesWeb.DbOperations;
using CarImagesWeb.Helpers;
using CarImagesWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarImagesWeb.ApiControllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AssetsApiController : Controller
    {
        private readonly IAssetRepository _assetRepository;

        public AssetsApiController(IAssetRepository assetRepository)
        {
            _assetRepository = assetRepository;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAssets()
        {
            var assets = await _assetRepository.GetAllAsync();
            var vehicles = new List<Asset>();
            var containers = new List<Asset>();
            // divide assets into vehicles and containers based on their type
            foreach (var asset in assets)
            {
                if (asset.Type == AssetType.Vehicle.ToString())
                {
                    vehicles.Add(asset);
                }
                else if (asset.Type == AssetType.Container.ToString())
                {
                    containers.Add(asset);
                }
            }
            
            return Json(new
            {
                data = new
                {
                    vehicles = vehicles, 
                    containers = containers
                }
            });
        }
    }
}