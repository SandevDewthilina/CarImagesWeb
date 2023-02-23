using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CarImagesWeb.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CarImagesWeb.Services
{
    public interface IBlobStorageHandler
    {
        Task UploadImageAsync(IFormFile file, ImageThumbnail imageThumbnail, string directoryName);
        Task<Stream> GetImageAsync(string imageName);
        Task DeleteImageAsync(string imageName);
        Task<List<string>> GetImagesInDirectoryAsync(string directoryName);

        Task UploadImagesAsync(IFormFileCollection files, List<ImageThumbnail> imageThumbnails, string assetDirectory);
    }

    public class BlobStorageHandler : IBlobStorageHandler
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _containerClient;
 
        public BlobStorageHandler(IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>("AzureStorage:ConnectionString");
            var containerName = configuration.GetValue<string>("AzureStorage:ContainerName");
            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        }

        private async Task UploadImageAsync(Stream imageStream, string imageName)
        {
            var blobClient = _containerClient.GetBlobClient(imageName);
            await blobClient.UploadAsync(imageStream);
        }

        public async Task UploadImageAsync(IFormFile file, ImageThumbnail imageThumbnail, string directoryName)
        {
            try
            {
                await UploadImageAsync(file.OpenReadStream(), directoryName + "/" + file.FileName);
            }
            catch(Azure.RequestFailedException e)
            {
                if (e.ErrorCode == "BlobAlreadyExists")
                {
                    throw new Azure.RequestFailedException(
                        e.Status,
                        "Image already exist: " + file.FileName
                    );
                }

                throw new Exception(
                    "An error occurred while uploading the image. Please try again later.");
            }

            await UploadImageAsync(imageThumbnail.GetStream(), directoryName + "/" + imageThumbnail.FileName);
        }

        public async Task<Stream> GetImageAsync(string imageName)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(imageName);
            var response = await blobClient.DownloadAsync();
            return response.Value.Content;
        }

        public async Task DeleteImageAsync(string imageName)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(imageName);
            await blobClient.DeleteAsync();
        }
        
        public async Task<List<string>> GetImagesInDirectoryAsync(string directoryName)
        {
            List<string> imageNames = new List<string>();
            await foreach (BlobHierarchyItem item in _containerClient.GetBlobsByHierarchyAsync(delimiter: "/", prefix: directoryName + "/"))
            {
                if (item.IsBlob)
                {
                    imageNames.Add(item.Blob.Name);
                }
            }
            return imageNames;
        }

        public Task UploadImagesAsync(IFormFileCollection files, List<ImageThumbnail> imageThumbnails, string assetDirectory)
        {
            throw new NotImplementedException();
        }
    }
    
    
}