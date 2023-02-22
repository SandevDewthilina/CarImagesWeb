﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CarImagesWeb.DbContext;
using CarImagesWeb.Models;
using CarImagesWeb.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CarImagesWeb.DbOperations
{
    public interface IImagesRepository : IRepository<ImageUpload>
    {
        Task SaveImagesAsync(IEnumerable<ImageUpload> imageUpload, IFormFileCollection files, string assetDirectory);
    }
    
    public class ImagesRepository : Repository<ImageUpload>, IImagesRepository
    {
        private readonly IBlobStorageHandler _blobStorageHandler;

        public ImagesRepository(CarImagesDbContext context, IBlobStorageHandler blobStorageHandler) : base(context)
        {
            _blobStorageHandler = blobStorageHandler;
        }

        public async Task SaveImagesAsync(IEnumerable<ImageUpload> imageUploads, IFormFileCollection files,
            string assetDirectory)
        {
            // Save the imageUploads to the database
            foreach (var imageUpload in imageUploads)
            {
                await AddAsync(imageUpload);
            }
            // Upload the images to blob storage
            await _blobStorageHandler.UploadImagesAsync(files, assetDirectory);
        }
    }
}