using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarImagesWeb.DbOperations;
using CarImagesWeb.Helpers;
using CarImagesWeb.Models;
using CarImagesWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;

namespace CarImagesWeb.ApiControllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TagsApiController : Controller
    {
        private readonly ITagsHandler _tagsHandler;
        private readonly IImagesRepository _imagesRepository;

        public TagsApiController(ITagsHandler tagsHandler, IImagesRepository imagesRepository)
        {
            _tagsHandler = tagsHandler;
            _imagesRepository = imagesRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            var userRoles = UserHelper.GetRolesOfUser(User);
            var tags = await _tagsHandler.GetTagsForRoles(userRoles, context: "Download");
            return Json(new
            {
                data = new
                {
                    vehicleTags = tags.Where(t => t.Type.Equals("Vehicle")),
                    containerTags = tags.Where(t => t.Type.Equals("Container"))
                }
            });
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetTagsForRoles(List<string> roles)
        {
            var userRoles = roles == null || !roles.Any() ? UserHelper.GetRolesOfUser(User) : roles;
            var tags = await _tagsHandler.GetTagsForRoles(userRoles);
            return Json(new
            {
                data = new
                {
                    tags
                }
            });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> GetTagsWithCountForVehicle(TagAssetFilterModel model)
        {
            var tagList = await _tagsHandler.GetTagsForIdsAsync(model.tagIds);
            foreach (Tag tag in tagList)
            {
                var imageUploads = await _imagesRepository.FindAsync(i => i.TagId == tag.Id && i.AssetId == model.assetId);
                tag.Name = tag.Name + $" ({imageUploads.Count} Uploads)";
            }

            return Json(new {success = true, data = tagList});
        }
    }

    public class TagAssetFilterModel
    {
        public List<int> tagIds { get; set; }
        public int assetId { get; set; }
    }
}