using System.Threading.Tasks;
using CarImagesWeb.DbOperations;
using CarImagesWeb.Models;
using CarImagesWeb.ViewModels;
using CarImagesWeb.ViewModels.TagViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CarImagesWeb.Controllers
{
    public class CountryController : Controller
    {
        private readonly ICountryRepository _countryRepository;

        public CountryController(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public IActionResult CreateCountry()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCountry(CreateEntityViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _countryRepository.AddAsync(new Country
            {
                Name = model.Name,
                Code = model.Code
            });

            return RedirectToAction("ListCountries", "Country");
        }

        public async Task<IActionResult> ListCountries()
        {
            var tags = await _countryRepository.GetAllAsync();
            return View(tags);
        }
        
        [HttpGet]
        public async Task<IActionResult> EditCountry(int id)
        {
            var tag = await _countryRepository.GetAsync(t => t.Id == id);
            if (tag is null) return NotFound();

            var model = new EditEntityViewModel
            {
                Id = tag.Id,
                Name = tag.Name,
                Code = tag.Code
            };

            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> EditCountry(EditEntityViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var country = await _countryRepository.GetAsync(t => t.Id == model.Id);
            if (country is null) return NotFound();

            country.Name = model.Name;

            await _countryRepository.UpdateAsync(country);

            return RedirectToAction("ListCountries", "Country");
        }
        
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await _countryRepository.GetAsync(t => t.Id == id);
            if (country is null) return NotFound();

            await _countryRepository.DeleteAsync(country);

            return RedirectToAction("ListCountries", "Country");
        }
    }
}