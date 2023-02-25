using System.Security.Claims;
using System.Threading.Tasks;
using CarImagesWeb.DbOperations;
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

        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var tags = await _tagsHandler.GetTagsForRole(userRole);
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