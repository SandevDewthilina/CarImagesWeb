using System;
using System.Threading.Tasks;
using CarImagesWeb.DTOs;
using Microsoft.AspNetCore.Http;

namespace CarImagesWeb.Services
{
    public interface IImagesHandler
    {
        Task HandleUpload(ImageUploadDto dto, IFormFileCollection files);
        Task HandleSearch();
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
    }
}