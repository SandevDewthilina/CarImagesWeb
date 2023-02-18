using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CarImagesWeb.Services
{
    public interface IBlobStorageHandler
    {
        Task UploadImages(string path, IFormFileCollection files);
    }

    public class BlobStorageHandler : IBlobStorageHandler
    {
        public Task UploadImages(string path, IFormFileCollection files)
        {
            throw new System.NotImplementedException();
        }
    }
    
    
}