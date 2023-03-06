using System.Threading.Tasks;
using CarImagesWeb.DbOperations;
using CarImagesWeb.Models;
using CarImagesWeb.ViewModels;
using CarImagesWeb.ViewModels.TagViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarImagesWeb.Controllers
{
    [Authorize]
    public class TagsController : Controller
    {
        private readonly ITagRepository _tagRepository;
        private readonly ICountryRepository _countryRepository;

        public TagsController(ITagRepository tagRepository, ICountryRepository countryRepository)
        {
            _tagRepository = tagRepository;
            _countryRepository = countryRepository;
        }

        public async Task<IActionResult> CreateTag()
        {
            return View(new CreateTagViewModel()
            {
                Countries = await _countryRepository.GetAllAsync()
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateTag(CreateTagViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _tagRepository.AddAsync(new Tag
            {
                Name = model.Name,
                Code = model.Code,
                CountryId = model.SelectedCountryId,
                Type = model.Type
            });

            return RedirectToAction("ListTags", "Tags");
        }

        public async Task<IActionResult> ListTags()
        {
            var tags = await _tagRepository.GetAllAsync();
            return View(tags);
        }
        
        [HttpGet]
        public async Task<IActionResult> EditTag(int id)
        {
            var tag = await _tagRepository.GetAsync(t => t.Id == id);
            if (tag is null) return NotFound();

            var model = new EditTagViewModel()
            {
                Id = tag.Id,
                Name = tag.Name,
                Code = tag.Code,
                SelectedCountryId = tag.CountryId,
                Type = tag.Type,
                Countries = await _countryRepository.GetAllAsync()
            };

            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> EditTag(EditTagViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var tag = await _tagRepository.GetAsync(t => t.Id == model.Id);
            if (tag is null) return NotFound();

            tag.Name = model.Name;
            tag.CountryId = model.SelectedCountryId;
            tag.Type = model.Type;

            await _tagRepository.UpdateAsync(tag);

            return RedirectToAction("ListTags", "Tags");
        }
        
        public async Task<IActionResult> DeleteTag(int id)
        {
            var tag = await _tagRepository.GetAsync(t => t.Id == id);
            if (tag is null) return NotFound();

            await _tagRepository.DeleteAsync(tag);

            return RedirectToAction("ListTags", "Tags");
        }
    }

    
}