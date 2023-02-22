using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarImagesWeb.DbContext;
using CarImagesWeb.DbOperations;
using CarImagesWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarImagesWeb.Controllers
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
                    vehicleTags = tags,
                    containerTags = tags
                }
            });
        }
    }
}