﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using CarImagesWeb.ApiControllers;
using CarImagesWeb.DbOperations;
using CarImagesWeb.DTOs;
using CarImagesWeb.Helpers;
using CarImagesWeb.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace CarImagesWeb.Services
{
    public interface IImagesHandler
    {
        /// <summary>
        /// </summary>
        /// <param name="thumbnailUrls"></param>
        /// <returns></returns>
        Task<byte[]> HandleDownloadFromThumbnails(IEnumerable<string> thumbnailUrls);

        Task<List<ImageUpload>> HandleSearch(string assetType, string assetId, List<string> tags, string countryCode);

        /// <summary>
        ///     Get the asset directory for the given asset code, country code and tag name
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
        Task DeleteUpload(ImageUpload upload);
        Task HandleExternalUpload(ExternalImageUploadDto dto);
    }

    public class ImagesHandler : IImagesHandler
    {
        private readonly IAssetsHandler _assetHandler;
        private readonly string _containerUrl;
        private readonly ICountryHandler _countryHandler;
        private readonly IAssetRepository _assetRepository;
        private readonly IVehicleContainerRepository _vehicleContainerRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IImagesRepository _imagesRepository;
        private readonly ITagsHandler _tagHandler;

        public ImagesHandler(IImagesRepository imagesRepository, IAssetsHandler assetHandler,
            ITagsHandler tagHandler, ICountryHandler countryHandler, IConfiguration configuration, 
            IAssetRepository assetRepository, IVehicleContainerRepository vehicleContainerRepository, ITagRepository tagRepository, IWebHostEnvironment webHostEnvironment)
        {
            _imagesRepository = imagesRepository;
            _assetHandler = assetHandler;
            _tagHandler = tagHandler;
            _countryHandler = countryHandler;
            _assetRepository = assetRepository;
            _vehicleContainerRepository = vehicleContainerRepository;
            _tagRepository = tagRepository;
            _webHostEnvironment = webHostEnvironment;
            var storageAccountName = configuration["AzureStorage:AccountName"];
            var containerName = configuration["AzureStorage:ContainerName"];
            _containerUrl = $"https://{storageAccountName}.blob.core.windows.net/{containerName}";
        }

        public async Task HandleUpload(ImageUploadDto dto)
        {
            await HandleUpload(dto, dto.File);
        }

        public async Task DeleteUpload(ImageUpload upload)
        {
            var filepath = GetAssetDirectory(upload.Asset, upload.Country, upload.Tag) + "/" + upload.FileName;
            var thumbFilepath = GetAssetDirectory(upload.Asset, upload.Country, upload.Tag) + "/" + GetThumbnailName(upload.FileName);
            await _imagesRepository.DeleteImageAsync(thumbFilepath);
            await _imagesRepository.DeleteImageAsync(filepath);
        }

        public async Task HandleExternalUpload(ExternalImageUploadDto dto)
        {
            var asset = await _assetRepository.GetAsync(a => a.Code.Equals(dto.AssetCode));
            var tag = await _tagRepository.GetAsync(t => t.Code.Equals(dto.TagCode));
            var country = await _countryHandler.GetCountryFromCode(dto.CountryCode);
            var assetDirectory = GetAssetDirectory(asset, country, tag);
            var filename = Guid.NewGuid() + Path.GetExtension(dto.File.FileName);
            
            var imageUpload = new ImageUpload
            {
                FileName = filename,
                Asset = asset,
                AssetId = asset.Id,
                Country = country,
                CountryId = country.Id,
                UserId = "",
                Tag = tag,
                TagId = tag.Id
            };

            var thumbnail = await CreateThumbnailAsync(dto.File, 200);
            var thumbFileName = GetThumbnailName(filename);
            var imageThumbnail = new ImageThumbnail
            {
                FileName = thumbFileName,
                File = thumbnail
            };

            await _imagesRepository.SaveImageAsync(imageUpload, dto.File, imageThumbnail, assetDirectory);
            
        }

        public async Task HandleUpload(ImageUploadDto dto, IFormFile file)
        {
            var asset = await _assetHandler.GetAssetToUpload(dto);
            var tag = await _tagHandler.GetTagToUpload(dto);
            var country = await _countryHandler.GetCountryFromCode(dto.CountryCode);
            var assetDirectory = GetAssetDirectory(asset, country, tag);
            var filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var imageUpload = new ImageUpload
            {
                FileName = filename,
                Asset = asset,
                AssetId = asset.Id,
                Country = country,
                CountryId = country.Id,
                UserId = "",
                Tag = tag,
                TagId = tag.Id
            };

            var thumbnail = await CreateThumbnailAsync(file, 200);
            var thumbFileName = GetThumbnailName(filename);
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
            var tag = await _tagHandler.GetTagToUpload(dto);
            var country = await _countryHandler.GetCountryFromCode(dto.CountryCode);

            var assetDirectory = GetAssetDirectory(asset, country, tag);

            var imageUploads = new List<ImageUpload>();

            // convert files to list
            // create thumbnails for each image
            var thumbnails = new List<ImageThumbnail>();
            foreach (var file in files)
            {
                var imageUpload = new ImageUpload
                {
                    FileName = Guid.NewGuid() + Path.GetExtension(file.FileName),
                    Asset = asset,
                    AssetId = asset.Id,
                    Country = country,
                    CountryId = country.Id,
                    UserId = "",
                    Tag = tag,
                    TagId = tag.Id
                };
                imageUploads.Add(imageUpload);
                var thumb = await CreateThumbnailAsync(file, 200);
                var thumbFileName = GetThumbnailName(imageUpload.FileName);
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

        public async Task<List<ImageUpload>> HandleSearch(string assetType, string assetId, List<string> tags,
            string countryCode)
        {
            // Get the asset from the database
            // var tagIds = tags.Select(int.Parse).ToList();
            var tagIds = new List<int>();
            tagIds = tags.Select(int.Parse).ToList();


            List<ImageUpload> uploads;
            Expression<Func<ImageUpload, bool>> expression;
            
            // if asset type is Vehicle allow both container and vehicle results
            bool isVehicle = assetType.ToLower().Equals(AssetType.Vehicle.ToString().ToLower());
            
            if (assetId != string.Empty && tagIds.Count == 0)
            {
                var asset = int.Parse(assetId);
                expression = i => i.AssetId == asset && i.Asset.Type == assetType;
                if(countryCode != string.Empty)
                {
                    // aggregate the countryCode to the expression
                    expression = i => i.AssetId == asset && i.Asset.Type == assetType
                                                         && i.Country.Code == countryCode;
                }
            }
            else if (assetId == string.Empty && tagIds.Count > 0)
            {
                expression = i => tagIds.Contains(i.TagId) &&  i.Asset.Type == assetType;
                if(countryCode != string.Empty)
                {
                    // aggregate the countryCode to the expression
                    expression = i => tagIds.Contains(i.TagId) &&   i.Asset.Type == assetType
                                                               && i.Country.Code == countryCode;
                }
            }
            else
            {
                var asset = int.Parse(assetId);
                expression = i => i.AssetId == asset && tagIds.Contains(i.TagId) 
                                                     &&  ( i.Asset.Type == assetType);
                if(countryCode != string.Empty)
                {
                    // aggregate the countryCode to the expression
                    expression = i => i.AssetId == asset && tagIds.Contains(i.TagId) 
                                                         &&  ( i.Asset.Type == assetType) && i.Country.Code == countryCode;
                }
            }
            
            uploads = await _imagesRepository.FindAsync(
                expression);

            // combine the container images
            if (isVehicle)
            {
                // get mapped containers
                var containerIdList = new List<int>();
                foreach (ImageUpload imageUpload in uploads)
                {
                    var asset = imageUpload.Asset;
                    var mappings = await _vehicleContainerRepository
                        .FindAsync(a => a.VehicleAssetId == asset.Id);
                    var containers = mappings.Select(m => m.ContainerAssetId);
                    containerIdList.AddRange(containers);
                }
                Expression<Func<ImageUpload, bool>> containerUploadsExpression = u => containerIdList.Contains(u.AssetId);
                var containerTagList = await _tagRepository.FindAsync(t => tagIds.Contains(t.Id) && t.Type.Equals("Container"));
                if (containerTagList?.Count > 0)
                {
                    containerUploadsExpression = u => containerIdList.Contains(u.AssetId) && tagIds.Contains(u.TagId);
                }
                

                // get image uploads for the containers
                var imageUploadsForContainer = await _imagesRepository
                    .FindAsync(containerUploadsExpression);
                if (imageUploadsForContainer != null)
                {
                    uploads.AddRange(imageUploadsForContainer);
                }
            }
            
            
            return uploads.ToList();
        }

        public async Task<byte[]> HandleDownloadFromThumbnails(IEnumerable<string> thumbnailUrls)
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

        private static async Task<Image> CreateThumbnailAsync(IFormFile file, int maxHeight, int quality = 100)
        {
            // Load the original image from the form file
            await using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);

            // Resize the image to a thumbnail with the specified max width
            using var image = await Image.LoadAsync(stream);
            var divisor = (float) image.Height / maxHeight;
            var size = new Size((int) (image.Width / divisor), (int) (image.Height / divisor));
            image.Mutate(x => x.Resize(size));

            // Convert the thumbnail image to a JPEG image with the specified quality
            var jpegEncoder = new JpegEncoder {Quality = quality};
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
            var jpegEncoder = new JpegEncoder {Quality = quality};
            using var stream = new MemoryStream();
            await image.SaveAsync(stream, jpegEncoder);
            stream.Seek(0, SeekOrigin.Begin);

            // Create a new IFormFile from the JPEG image stream
            var formFile = new FormFile(stream, 0, stream.Length, fileName, fileName);
            return formFile;
        }
    }
}