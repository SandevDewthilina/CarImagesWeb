using System.Threading.Tasks;
using CarImagesWeb.DbOperations;
using CarImagesWeb.Models;
using CarImagesWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CarImagesWeb.Controllers
{
    public class TagsController : Controller
    {
        private readonly ITagRepository _tagRepository;

        public TagsController(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }
        
        public IActionResult CreateTag()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateTag(CreateTagViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            
            var result= await _tagRepository.AddAsync(new Tag()
            {
                Name = model.Name,
                Code = model.Code
            });
                
            return RedirectToAction("ListTags", "Tags");

        }

        public async Task<IActionResult> ListTags()
        {
            var tags = await _tagRepository.GetAllAsync();
            return View(tags);
        }
    }
}