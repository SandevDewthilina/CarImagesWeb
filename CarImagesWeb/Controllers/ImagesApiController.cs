using System;
using System.Threading.Tasks;
using CarImagesWeb.DTOs;
using CarImagesWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarImagesWeb.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ImagesApiController : Controller
    {
        private readonly IImagesHandler _imagesHandler;

        public ImagesApiController(IImagesHandler imagesHandler)
        {
            _imagesHandler = imagesHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm]ImageUploadDto dto)
        {
            try
            {
                await _imagesHandler.HandleUpload(dto, Request.Form.Files);
            }
            catch
            {
                return Problem();
            }
            return Ok();
        }
    }
}