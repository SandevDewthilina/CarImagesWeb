using System.Linq;
using System.Threading.Tasks;
using CarImagesWeb.Helpers;
using CarImagesWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarImagesWeb.ApiControllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CountryApiController : Controller
    {
        private readonly ICountryHandler _countryHandler;

        public CountryApiController(ICountryHandler countryHandler)
        {
            _countryHandler = countryHandler;
        }

        [HttpGet]
        public async Task<IActionResult> GetCountries()
        {
            var countries = await _countryHandler.GetCountryCodesAsync();
            return Json(new
            {
                data = countries
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetCountriesForUser()
        {
            var userRoles = UserHelper.GetRolesOfUser(User);
            var countries = await _countryHandler.GetCountryForRoles(userRoles);
            return Json(new
            {
                data = countries.Select(c => c.Code)
            });
        }
    }
}