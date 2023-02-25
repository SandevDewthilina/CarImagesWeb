using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CarImagesWeb.Services;
using CarImagesWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarImagesWeb.Controllers
{
    [Authorize]
    public class ImagesController : Controller
    {
        private readonly IAssetsHandler _assetsHandler;
        private readonly ICountryHandler _countryHandler;
        private readonly ITagsHandler _tagsHandler;

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
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var viewModel = new ImageUploadViewModel
            {
                Vehicles = await _assetsHandler.GetVehiclesAsync(),
                Containers = await _assetsHandler.GetContainersAsync(),
                Tags = await _tagsHandler.GetTagsForRole(userRole),
                CountryCodes = await _countryHandler.GetCountryCodesAsync()
            };
            return View(viewModel);
        }

        public IActionResult Gallery()
        {
            return View();
        }
    }
}