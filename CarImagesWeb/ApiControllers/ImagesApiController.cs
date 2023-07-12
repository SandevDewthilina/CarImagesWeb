using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using CarImagesWeb.DbOperations;
using CarImagesWeb.DTOs;
using CarImagesWeb.Models;
using CarImagesWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarImagesWeb.ApiControllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ImagesApiController : Controller
    {
        private readonly IImagesHandler _imagesHandler;
        private readonly IImagesRepository _imagesRepository;
        private readonly IAssetRepository _assetRepository;
        private readonly ITagRepository _tagRepository;

        public ImagesApiController(IImagesHandler imagesHandler,
            IImagesRepository imagesRepository, 
            IAssetRepository assetRepository,
            ITagRepository tagRepository)
        {
            _imagesHandler = imagesHandler;
            _imagesRepository = imagesRepository;
            _assetRepository = assetRepository;
            _tagRepository = tagRepository;
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

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ExternalUpload([FromForm] ExternalImageUploadDto model)
        {
            try
            {
                await _imagesHandler.HandleExternalUpload(model);
            }
            catch (RequestFailedException e)
            {
                Response.StatusCode = e.Status;
                return Json(new {error = e.Message});
            }

            return Json(new {success = true});
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
            var countryCode = dto.Country;

            // find imageUploads by assetType and assetId and/or tags
            var imageUploads = await _imagesHandler.HandleSearch(assetType, assetId, tags, countryCode);

            // make list of anonymous objects of imageUploads and their thumbnails
            var data = new List<object>();
            foreach (var imageUpload in imageUploads)
            {
                var thumbnail = _imagesHandler.GetImageThumbnailUrl(imageUpload);
                data.Add(new
                {
                    uploadId = imageUpload.Id,
                    url = thumbnail,
                    imageData = new
                    {
                        country = imageUpload.Country.Name,
                        asset = imageUpload.Asset.Name,
                        tag = imageUpload.Tag.Name,
                        assetInfo = imageUpload.Asset
                    },
                    downloadUrl = _imagesHandler.GetImageUrlFromThumbnail(thumbnail)
                });
            }

            // await _imagesHandler.HandleSearch();
            return Json(new {data = data});
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAssetImages(string assetCode, string tagCode)
        {
            try
            {
                var asset = await _assetRepository.GetAsync(a => a.Code.Equals(assetCode));
                if (asset == null)
                {
                    throw new Exception("No Asset Found");
                }
                List<ImageUpload> imageUploads;
                if (string.IsNullOrEmpty(tagCode))
                {
                    imageUploads = await _imagesRepository.FindAsync(u => u.AssetId == asset.Id);
                }
                else
                {
                    var tag = await _tagRepository.GetAsync(t => t.Code.Equals(tagCode));
                    if (tag == null)
                    {
                        throw new Exception("Invalid Tag Code");
                    }
                    imageUploads = await _imagesRepository.FindAsync(u => u.AssetId == asset.Id && u.TagId == tag.Id);
                }
                var data = imageUploads.Select(imageUpload => _imagesHandler
                        .GetImageThumbnailUrl(imageUpload))
                    .Select(thumbnail => _imagesHandler.GetImageUrlFromThumbnail(thumbnail).Replace("\\", "/")).ToList();

                return Json(new
                {
                    success = true,
                    data = data
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    success = false,
                    message = e.Message
                });
            }
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

        [HttpGet]
        public async Task<IActionResult> DeleteUpload(int uploadId)
        {
            var upload = await _imagesRepository.GetAsync(iu => iu.Id == uploadId);
            _imagesHandler.DeleteUpload(upload).Wait();
            await _imagesRepository.DeleteAsync(upload);

            return Json(new {success = true});
        }
    }

    public class ExternalImageUploadDto
    {
        public string ImageCategory { get; set; }
        public string CountryCode { get; set; }
        public string AssetCode { get; set; }
        public string TagCode { get; set; }
        public IFormFile File { get; set; }
        public IFormFileCollection Files { get; set; }
    }
}