using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using CarImagesWeb.DTOs;
using CarImagesWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarImagesWeb.ApiControllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ImagesApiController : Controller
    {
        private readonly IImagesHandler _imagesHandler;

        public ImagesApiController(IImagesHandler imagesHandler)
        {
            _imagesHandler = imagesHandler;
        }

        /// <summary>
        /// This endpoint is used to upload a single image.
        /// </summary>
        /// <param name="dto">Contains the asset and tags associated with the image.</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] ImageUploadDto dto)
        {
            try
            {
                await _imagesHandler.HandleUpload(dto);
            }
            catch (RequestFailedException e)
            {
                Response.StatusCode = e.Status;
                return Json(new {error = e.Message});
            }

            return Ok();
        }

        /// <summary>
        /// This endpoint is used to upload multiple images.
        /// </summary>
        /// <param name="dto">Contains the asset and tag associated with the image.</param>
        /// <returns></returns>
        [Obsolete("This method is still to be developed, please use Upload instead.")]
        [HttpPost]
        public async Task<IActionResult> Uploads([FromForm] ImageUploadDto dto)
        {
            try
            {
                await _imagesHandler.HandleUpload(dto, Request.Form.Files);
            }
            catch (RequestFailedException e)
            {
                Response.StatusCode = e.Status;
                return Json(new {error = e.Message});
            }

            return Ok();
        }


        /// <summary>
        /// This endpoint is used to search for images.
        /// </summary>
        /// <param name="dto">Contain the asset type, asset and tags from which the search is to be done.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Search(ImageSearchDto dto)
        {
            var assetType = dto.AssetType;
            var assetId = dto.Asset;
            var tags = dto.Tags;
            var countryId = dto.Country;

            // find imageUploads by assetType and assetId and/or tags
            var imageUploads = await _imagesHandler.HandleSearch(assetType, assetId, tags);
            
            // make list of anonymous objects of imageUploads and their thumbnails
            var data = new List<object>();
            foreach (var imageUpload in imageUploads)
            {
                var thumbnail = _imagesHandler.GetImageThumbnailUrl(imageUpload);
                data.Add(new
                {
                    url = thumbnail,
                    imageData = new
                    {
                        country = imageUpload.Country.Name,
                        asset = imageUpload.Asset.Name,
                        tag = imageUpload.Tag.Name,
                    },
                    downloadUrl = _imagesHandler.GetImageUrlFromThumbnail(thumbnail)
                });
            }
            
            // await _imagesHandler.HandleSearch();
            return Json(new {data = data });
        }

        /// <summary>
        /// This endpoint is used to download images from the associated thumbnails.
        /// </summary>
        /// <param name="thumbnailUrls">This array contain the thumbnail urls.</param>
        [HttpPost]
        public async Task Download([FromBody] string[] thumbnailUrls)
        {
            //wait for 10 seconds
            var fileBytes = await _imagesHandler.HandleDownloadFromThumbnails(thumbnailUrls);
            //response
            Response.ContentType = "application/zip";
            Response.Headers.Add("Content-Disposition", "attachment; filename='images.zip'");
            await Response.Body.WriteAsync(fileBytes, 0, fileBytes.Length);
        }
    }
}