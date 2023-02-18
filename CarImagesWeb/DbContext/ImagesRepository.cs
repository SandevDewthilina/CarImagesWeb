using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace CarImagesWeb.DbContext
{
    public interface IImagesRepository
    {
        Task SaveCarImages();
        Task SaveContainerImages();
        
        Task UploadImageAsync(Stream imageStream, string imageName);
        Task<Stream> GetImageAsync(string imageName);
        Task DeleteImageAsync(string imageName);
        Task<List<string>> GetImagesInDirectoryAsync(string directoryName);
    }
    
    public class ImagesRepository : IImagesRepository
    {
        
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _containerClient;

        public ImagesRepository(string connectionString, string containerName)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        }

        public async Task UploadImageAsync(Stream imageStream, string imageName)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(imageName);
            await blobClient.UploadAsync(imageStream);
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
        
        public Task SaveCarImages()
        {
            throw new System.NotImplementedException();
        }

        public Task SaveContainerImages()
        {
            throw new System.NotImplementedException();
        }
    }
}