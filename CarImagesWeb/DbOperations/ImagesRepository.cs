using System.Collections.Generic;
using System.Threading.Tasks;
using CarImagesWeb.DbContext;
using CarImagesWeb.Helpers;
using CarImagesWeb.Models;
using CarImagesWeb.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;

namespace CarImagesWeb.DbOperations
{
    public interface IImagesRepository : IRepository<ImageUpload>
    {
        Task SaveImagesAsync(IEnumerable<ImageUpload> imageUpload, IFormFileCollection files,
            List<ImageThumbnail> imageThumbnails, string assetDirectory);

        Task SaveImageAsync(ImageUpload imageUpload, IFormFile file, ImageThumbnail thumbnail, string assetDirectory);
    }
    
    public class ImagesRepository : Repository<ImageUpload>, IImagesRepository
    {
        private readonly IBlobStorageHandler _blobStorageHandler;

        public ImagesRepository(CarImagesDbContext context, IBlobStorageHandler blobStorageHandler) : base(context)
        {
            _blobStorageHandler = blobStorageHandler;
        }

        //TODO: Refactor this method
        public async Task SaveImagesAsync(IEnumerable<ImageUpload> imageUploads, IFormFileCollection files,
            List<ImageThumbnail> imageThumbnails,
            string assetDirectory)
        {
            // Save the imageUploads to the database
            foreach (var imageUpload in imageUploads)
            {
                await AddAsync(imageUpload);
            }
            // Upload the images to blob storage
            await _blobStorageHandler.UploadImagesAsync(files, imageThumbnails, assetDirectory);
        }

        public async Task SaveImageAsync(ImageUpload imageUpload, IFormFile file, ImageThumbnail thumbnail,
            string assetDirectory)
        {
            await AddAsync(imageUpload);
            await _blobStorageHandler.UploadImageAsync(file, thumbnail, assetDirectory);
        }
    }
}