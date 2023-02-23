using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CarImagesWeb.DbOperations;
using CarImagesWeb.DTOs;
using CarImagesWeb.Helpers;
using CarImagesWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace CarImagesWeb.Services
{
    public interface IImagesHandler
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="thumbnailUrls"></param>
        /// <returns></returns>
        Task<byte[]> HandleDownload(IEnumerable<string> thumbnailUrls);

        Task<List<string>> HandleSearch(string assetType, string assetId, List<string> tags);
        
        /// <summary>
        /// Get the asset directory for the given asset code, country code and tag name
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="country"></param>
        /// <param name="tag"></param>
        /// <returns>string: Path to the directory in which the asset is stored</returns>
        string GetAssetDirectory(Asset asset, Country country, Tag tag);
        string GetImageThumbnailUrl(ImageUpload imageUpload);
        string GetThumbnailName(string fileName);
        string GetImageUrlFromThumbnail(string thumbnailUrl);
        Task HandleUpload(ImageUploadDto dto, IFormFile file);
        Task HandleUpload(ImageUploadDto dto, IFormFileCollection files);

        Task HandleUpload(ImageUploadDto dto);
    }
    public class ImagesHandler : IImagesHandler
    {
        private readonly IImagesRepository _imagesRepository;
        private readonly IAssetsHandler _assetHandler;
        private readonly ITagsHandler _tagHandler;
        private readonly ICountryHandler _countryHandler;
        private readonly string _containerUrl;

        public ImagesHandler(IImagesRepository imagesRepository, IAssetsHandler assetHandler, 
            ITagsHandler tagHandler, ICountryHandler countryHandler, IConfiguration configuration)
        {
            _imagesRepository = imagesRepository;
            _assetHandler = assetHandler;
            _tagHandler = tagHandler;
            _countryHandler = countryHandler;
            var storageAccountName = configuration["AzureStorage:AccountName"];
            var containerName = configuration["AzureStorage:ContainerName"];
            _containerUrl = $"https://{storageAccountName}.blob.core.windows.net/{containerName}";
        }

        public async Task HandleUpload(ImageUploadDto dto)
        {
            await HandleUpload(dto, dto.File);
        }
        
        public async Task HandleUpload(ImageUploadDto dto, IFormFile file)
        {
            var asset = await _assetHandler.GetAssetToUpload(dto);
            var tag = await  _tagHandler.GetTagToUpload(dto);
            var country = await _countryHandler.GetCountryFromCode(dto.CountryCode);
            var assetDirectory = GetAssetDirectory(asset, country, tag);
            var imageUpload = new ImageUpload
            {
                FileName = file.FileName,
                Asset = asset,
                AssetId = asset.Id,
                Country = country,
                CountryId = country.Id,
                UserId = "",
                Tag = tag,
                TagId = tag.Id
            };
            
            var thumbnail = await CreateThumbnailAsync(file, 200);
            var thumbFileName = GetThumbnailName(file.FileName);
            var imageThumbnail = new ImageThumbnail
            {
                FileName = thumbFileName,
                File = thumbnail
            };
            
            await _imagesRepository.SaveImageAsync(imageUpload, file, imageThumbnail, assetDirectory);
            
        }
        
        //TODO: Refactor this method
        public async Task HandleUpload(ImageUploadDto dto, IFormFileCollection files)
        {
            var asset = await _assetHandler.GetAssetToUpload(dto);
            var tag = await  _tagHandler.GetTagToUpload(dto);
            var country = await _countryHandler.GetCountryFromCode(dto.CountryCode);
            
            var assetDirectory = GetAssetDirectory(asset, country, tag);
            
            var imageUploads = files.Select(file => new ImageUpload
                {
                    FileName = file.FileName,
                    Asset = asset,
                    AssetId = asset.Id,
                    Country = country,
                    CountryId = country.Id,
                    UserId = "",
                    Tag = tag,
                    TagId = tag.Id
                })
                .ToList();
            
             // convert files to list
             // create thumbnails for each image
             var thumbnails = new List<ImageThumbnail>();
             foreach (var file in files)
             {
                 var thumb = await CreateThumbnailAsync(file, 200);
                 var thumbFileName = GetThumbnailName(file.FileName);
                    thumbnails.Add(new ImageThumbnail
                    {
                        FileName = thumbFileName,
                        File = thumb
                    });
             }
            
            await _imagesRepository.SaveImagesAsync(imageUploads, files, thumbnails, assetDirectory);
        }

        public string GetImageThumbnailUrl(ImageUpload imageUpload)
        {
            var assetDirectory = GetAssetDirectory(imageUpload.Asset, imageUpload.Country, imageUpload.Tag);
            return $"{_containerUrl}/{assetDirectory}/{GetThumbnailName(imageUpload.FileName)}";
        }

        public async Task<List<string>> HandleSearch(string assetType, string assetId, List<string> tags)
        {
            // Get the asset from the database
            var asset = int.Parse(assetId);
            var tagIds = tags.Select(int.Parse).ToList();
            List<ImageUpload> uploads;
            if (asset > 0 && tagIds.Count == 0)
            {
                uploads = await _imagesRepository.FindAsync(
                    i => i.AssetId == asset);
            }
            else if (asset == 0 && tagIds.Count > 0)
            {
                uploads = await _imagesRepository.FindAsync(
                    i => tagIds.Contains(i.TagId));
            }
            else
            {
                uploads = await _imagesRepository.FindAsync(
                    i => i.AssetId == asset && tagIds.Contains(i.TagId));
            }
            
            return uploads.Select(GetImageThumbnailUrl).ToList();
        }

        public async Task<byte[]> HandleDownload(IEnumerable<string> thumbnailUrls)
        {
            // Create a new memory stream for the zip file
            using var memoryStream = new MemoryStream();
            // Create a new zip archive in the memory stream
            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                // Loop through each selected image and add it to the zip archive
                foreach (var thumbnailUrl in thumbnailUrls)
                {
                    var imageUrl = GetImageUrlFromThumbnail(thumbnailUrl);
                    // Download the image from the URL
                    await using var imageStream = new WebClient().OpenRead(imageUrl);
                    // Add the image to the zip archive
                    var zipEntry = zipArchive.CreateEntry(Path.GetFileName(imageUrl), CompressionLevel.Optimal);
                    await using var entryStream = zipEntry.Open();
                    await imageStream!.CopyToAsync(entryStream);
                }
            }

            // Send the zip file as a file content response
            var bytes = memoryStream.ToArray();
            return bytes;
        }

        public string GetAssetDirectory(Asset asset, Country country, Tag tag)
        {
            return Path.Combine(asset.Type, asset.Code, country.Code, tag.Name);
        }

        private static async Task<Image> CreateThumbnailAsync(IFormFile file, int maxHeight, int quality = 100)
        {
            // Load the original image from the form file
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);

            // Resize the image to a thumbnail with the specified max width
            using var image = await Image.LoadAsync(stream);
            var divisor = (float)image.Height / maxHeight;
            var size = new Size((int)(image.Width / divisor), (int)(image.Height / divisor));
            image.Mutate(x => x.Resize(size));

            // Convert the thumbnail image to a JPEG image with the specified quality
            var jpegEncoder = new JpegEncoder { Quality = quality };
            var thumbnailStream = new MemoryStream();
            await image.SaveAsync(thumbnailStream, jpegEncoder);
            thumbnailStream.Seek(0, SeekOrigin.Begin);

            // Create a new Image<Rgba32> object from the JPEG image stream
            var thumbnail = await Image.LoadAsync(thumbnailStream);
            return thumbnail;
        }

        private static async Task<FormFile> CreateThumbnailFileAsync(Image image, string fileName, int quality = 80)
        {
            // Convert the image to a JPEG image with the specified quality
            var jpegEncoder = new JpegEncoder { Quality = quality };
            using var stream = new MemoryStream();
            await image.SaveAsync(stream, jpegEncoder);
            stream.Seek(0, SeekOrigin.Begin);

            // Create a new IFormFile from the JPEG image stream
            var formFile = new FormFile(stream, 0, stream.Length, fileName, fileName);
            return formFile;
        }
        
        public string GetThumbnailName(string fileName)
        {
            return Path.GetFileNameWithoutExtension(fileName) + "_thumb" + Path.GetExtension(fileName);
        }

        public string GetImageUrlFromThumbnail(string thumbnailUrl)
        {
            //remove the final '_thumb' part of the file name
            var thumbnailFileName = Path.GetFileNameWithoutExtension(thumbnailUrl);
            var index = thumbnailFileName.LastIndexOf("_thumb", StringComparison.Ordinal);
            var fileName = thumbnailFileName[..index];
            //replace the thumbnail url with the original image url
            return thumbnailUrl.Replace(thumbnailFileName, fileName);
        }
    }
}