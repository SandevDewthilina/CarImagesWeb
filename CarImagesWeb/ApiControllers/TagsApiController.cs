using System.Threading.Tasks;
using CarImagesWeb.DbOperations;
using Microsoft.AspNetCore.Mvc;

namespace CarImagesWeb.ApiControllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TagsApiController : Controller
    {
        private readonly ITagRepository _tagRepository;

        public TagsApiController(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _tagRepository.GetAllAsync();
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