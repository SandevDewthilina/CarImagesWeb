using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using CarImagesWeb.DTOs;
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
        private readonly IBlobStorageHandler _blobStorageHandler;

        public ImagesHandler(IBlobStorageHandler blobStorageHandler)
        {
            _blobStorageHandler = blobStorageHandler;
        }
        public async Task HandleUpload(ImageUploadDto dto, IFormFileCollection files)
        {
            //print file names
            foreach (var file in files)
            {
                Console.WriteLine(file.FileName);
            }
            
            //print out the model properties using system reflection
            var properties = dto.GetType().GetProperties();
            foreach (var property in properties)
            {
                Console.WriteLine($"{property.Name}: {property.GetValue(dto)}");
            }
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