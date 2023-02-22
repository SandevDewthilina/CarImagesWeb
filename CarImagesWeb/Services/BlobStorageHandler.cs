using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CarImagesWeb.Services
{
    public interface IBlobStorageHandler
    {
        Task UploadImageAsync(Stream imageStream, string imageName);
        Task UploadImagesAsync(IFormFileCollection files, string directoryName);
        Task<Stream> GetImageAsync(string imageName);
        Task DeleteImageAsync(string imageName);
        Task<List<string>> GetImagesInDirectoryAsync(string directoryName);

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
        
        public async Task UploadImageAsync(Stream imageStream, string imageName)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(imageName);
            await blobClient.UploadAsync(imageStream);
        }

        public async Task UploadImagesAsync(IFormFileCollection files, string directoryName)
        {
            // upload images to blob storage
            foreach (var file in files)
            {
                await UploadImageAsync(file.OpenReadStream(), directoryName + "/" + file.FileName);
            }
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
    }
    
    
}