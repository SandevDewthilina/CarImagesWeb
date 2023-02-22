using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using CarImagesWeb.DbContext;
using CarImagesWeb.DbOperations;
using CarImagesWeb.DTOs;
using CarImagesWeb.Helpers;
using CarImagesWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarImagesWeb.Services
{
    public interface IImagesHandler
    {
        Task HandleUpload(ImageUploadDto dto, IFormFileCollection files);
        Task HandleSearch();
        Task<byte[]> HandleDownload(IEnumerable<string> imageUrls);
    }
    public class ImagesHandler : IImagesHandler
    {
        private readonly IImagesRepository _imagesRepository;

        public ImagesHandler(IImagesRepository imagesRepository)
        {
            _imagesRepository = imagesRepository;
        }
        public async Task HandleUpload(ImageUploadDto dto, IFormFileCollection files)
        {
            List<ImageUpload> imageUploads = new List<ImageUpload>();
            foreach (var file in files)
            {
                var imageUpload = new ImageUpload
                {
                    FileName = file.FileName,
                    Asset = new Asset(), //TODO: find the asset
                    AssetId = 0, //TODO: fill in the asset id
                    UserId = "", //TODO: fill in the user id
                    TagId = 0, //TODO: fill in the tag id
                    Tag = null //TODO: find the tag
                };
                imageUploads.Add(imageUpload);
            }
            await _imagesRepository.SaveImagesAsync(imageUploads, files);
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
                foreach (string imageUrl in imageUrls)
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
    }
}