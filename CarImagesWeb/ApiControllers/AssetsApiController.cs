using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CarImagesWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AssetsApiController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetAssets()
        {
            return Json(new
            {
                data = new
                {
                    vehicles = new List<string>() {"vehicle1", "vehicle2", "vehicle3"}, 
                    containers = new List<string>(){ "container1", "container2", "container3" }
                }
            });
        }
    }
}