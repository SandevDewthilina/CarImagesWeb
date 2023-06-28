using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarImagesWeb.DbOperations;
using CarImagesWeb.Helpers;
using CarImagesWeb.Models;
using CarImagesWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarImagesWeb.ApiControllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AssetsApiController : Controller
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IAssetsHandler _assetsHandler;

        public AssetsApiController(IAssetRepository assetRepository, IAssetsHandler assetsHandler)
        {
            _assetRepository = assetRepository;
            _assetsHandler = assetsHandler;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAssets()
        {
            var assets = await _assetRepository.GetAllAsync();
            var vehicles = new List<Asset>();
            var containers = new List<Asset>();
            // divide assets into vehicles and containers based on their type
            foreach (var asset in assets)
                if (asset.Type.ToLower() == AssetType.Vehicle.ToString().ToLower())
                    vehicles.Add(asset);
                else if (asset.Type.ToLower() == AssetType.Container.ToString().ToLower()) containers.Add(asset);

            return Json(new
            {
                data = new
                {
                    vehicles, containers
                }
            });
        }

        [HttpGet]
        public async Task<IActionResult> FilterAssets(string startDate, string endDate, string code)
        {
            var sd = DateTime.Parse(startDate);
            var ed = DateTime.Parse(endDate);
            return Json(new {success = true, 
                data = await _assetRepository
                    .GetAllAsync(
                        a => a.YardInDate.Date >= sd.Date && a.YardInDate.Date <= ed
                        )
            });
        }
    }
}