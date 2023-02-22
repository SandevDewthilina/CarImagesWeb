using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarImagesWeb.Services;
using CarImagesWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CarImagesWeb.Controllers
{
    public class ImagesController : Controller
    {
        private readonly IAssetsHandler _assetsHandler;
        private readonly ITagsHandler _tagsHandler;
        private readonly ICountryHandler _countryHandler;

        public ImagesController(IAssetsHandler assetsHandler, 
            ITagsHandler tagsHandler, ICountryHandler countryHandler)
        {
            _assetsHandler = assetsHandler;
            _tagsHandler = tagsHandler;
            _countryHandler = countryHandler;
        }
        
        [HttpGet]
        public async Task<IActionResult> Upload()
        {
            //TODO: optimize this
            var viewModel = new ImageUploadViewModel()
            {
                Vehicles = await _assetsHandler.GetVehiclesAsync(),
                Containers = await _assetsHandler.GetContainersAsync(),
                Tags = await _tagsHandler.GetTagsAsync(),
                CountryCodes = await _countryHandler.GetCountryCodesAsync(),
            };
            return View(viewModel);
        }
        
        public IActionResult Gallery()
        {
            return View();
        }
    }


}