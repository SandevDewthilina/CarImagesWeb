using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarImagesWeb.Helpers;
using CarImagesWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarImagesWeb.ApiControllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TagsApiController : Controller
    {
        private readonly ITagsHandler _tagsHandler;

        public TagsApiController(ITagsHandler tagsHandler)
        {
            _tagsHandler = tagsHandler;
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
    }
}