using System.Linq;
using System.Threading.Tasks;
using CarImagesWeb.DbOperations;
using CarImagesWeb.Helpers;
using CarImagesWeb.Models;
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
        private readonly IImagesRepository _imagesRepository;
        private readonly ITagsHandler _tagsHandler;

        public ImagesController(IAssetsHandler assetsHandler,
            ITagsHandler tagsHandler, ICountryHandler countryHandler, IImagesRepository imagesRepository)
        {
            _assetsHandler = assetsHandler;
            _tagsHandler = tagsHandler;
            _countryHandler = countryHandler;
            _imagesRepository = imagesRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Upload()
        {
            var userRoles = UserHelper.GetRolesOfUser(User);
            var tagList = await _tagsHandler.GetTagsForRoles(userRoles, context: "Upload");
            var countries = await _countryHandler.GetCountryForRoles(userRoles);
            foreach (Tag tag in tagList)
            {
                var imageUploads = await _imagesRepository.FindAsync(u => u.TagId == tag.Id);
                tag.Name = tag.Name + $"({imageUploads.Count} Uploads)";
            }
            var viewModel = new ImageUploadViewModel
            {
                Vehicles = await _assetsHandler.GetVehiclesAsync(),
                Containers = await _assetsHandler.GetContainersAsync(),
                VehicleTags = tagList.Where(t => t.Type.Equals("Vehicle")),
                ContainerTags = tagList.Where(t => t.Type.Equals("Container")),
                CountryCodes = countries.Select(c => c.Code)
            };
            return View(viewModel);
        }

        public IActionResult Gallery()
        {
            return View();
        }
    }
}