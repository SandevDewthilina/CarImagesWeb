using System.Collections.Generic;
using System.Threading.Tasks;
using CarImagesWeb.DTOs;
using CarImagesWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarImagesWeb.ApiControllers
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
        

        //TODO: add parameter model and update the api method and api call
        [HttpPost]
        public async Task<IActionResult> Search()
        {
            // await _imagesHandler.HandleSearch();
            return Json(new { data = new List<string>()
                {
                    "http://localhost:5000/dist/img/photo1.png", 
                    "http://localhost:5000/dist/img/photo2.png",
                } 
            });
        }
        
        [HttpPost]
        public async Task Download([FromBody] string[] imageUrls)
        {
            //wait for 10 seconds
            var fileBytes = await _imagesHandler.HandleDownload(imageUrls);
            //response
            Response.ContentType = "application/zip";
            Response.Headers.Add("Content-Disposition", "attachment; filename='images.zip'");
            await Response.Body.WriteAsync(fileBytes, 0, fileBytes.Length);
        }

        
    }
}