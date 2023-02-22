using System.Threading.Tasks;
using CarImagesWeb.DbOperations;
using CarImagesWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarImagesWeb.ApiControllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TagsApiController : Controller
    {
        private readonly IRepository<Tag> _tagRepository;

        public TagsApiController(IRepository<Tag> tagRepository)
        {
            _tagRepository =  tagRepository;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _tagRepository.GetAllAsync();
            return Json(new
            {
                data = new
                {
                    tags = tags
                }
            });
        }
    }
}