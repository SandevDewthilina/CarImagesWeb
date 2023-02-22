using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CarImagesWeb.DbOperations;
using CarImagesWeb.DTOs;
using CarImagesWeb.Models;
using Microsoft.AspNetCore.Http;

namespace CarImagesWeb.Services
{
    public interface IImagesHandler
    {
        Task HandleUpload(ImageUploadDto dto, IFormFileCollection files);
        Task HandleSearch();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageUrls"></param>
        /// <returns></returns>
        Task<byte[]> HandleDownload(IEnumerable<string> imageUrls);
        
        /// <summary>
        /// Get the asset directory for the given asset code, country code and tag name
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="country"></param>
        /// <param name="tag"></param>
        /// <returns>string: Path to the directory in which the asset is stored</returns>
        string GetAssetDirectory(Asset asset, Country country, Tag tag);
    }
    public class ImagesHandler : IImagesHandler
    {
        private readonly IImagesRepository _imagesRepository;
        private readonly IAssetsHandler _assetHandler;
        private readonly ITagsHandler _tagHandler;
        private readonly ICountryHandler _countryHandler;

        public ImagesHandler(IImagesRepository imagesRepository, IAssetsHandler assetHandler, 
            ITagsHandler tagHandler, ICountryHandler countryHandler)
        {
            _imagesRepository = imagesRepository;
            _assetHandler = assetHandler;
            _tagHandler = tagHandler;
            _countryHandler = countryHandler;
        }
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
            
            await _imagesRepository.SaveImagesAsync(imageUploads, files, assetDirectory);
        }

        public Task HandleSearch()
        {
            throw new NotImplementedException();
        }

        public async Task<byte[]> HandleDownload(IEnumerable<string> imageUrls)
        {
            // Create a new memory stream for the zip file
            using var memoryStream = new MemoryStream();
            // Create a new zip archive in the memory stream
            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                // Loop through each selected image and add it to the zip archive
                foreach (var imageUrl in imageUrls)
                {
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
    }
}