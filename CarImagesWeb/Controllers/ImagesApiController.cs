using System;
using System.Collections.Generic;
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
        
        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            return Json(new
            {
                data = new
                {
                    vehicleTags = new List<string>(){"tag1", "tag2", "tag3"}, 
                    containerTags = new List<string>(){ "tag4", "tag5", "tag6"}
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Search()
        {
            // await _imagesHandler.HandleSearch();
            return Json(new { data = new List<string>()
                {
                    "http://localhost:5000/dist/img/photo1.png", 
                    "http://localhost:5000/dist/img/photo2.png", 
                    "image3"
                } 
            });
        }
    }
}