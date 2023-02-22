using System.Collections.Generic;
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
        Task SaveImagesAsync(IEnumerable<ImageUpload> imageUpload, IFormFileCollection files);
    }
    
    public class ImagesRepository : Repository<ImageUpload>, IImagesRepository
    {
        private readonly CarImagesDbContext _context;
        private readonly IBlobStorageHandler _blobStorageHandler;

        public ImagesRepository(CarImagesDbContext context, IBlobStorageHandler blobStorageHandler) : base(context)
        {
            _blobStorageHandler = blobStorageHandler;
        }

        public async Task SaveImagesAsync(IEnumerable<ImageUpload> imageUploads, IFormFileCollection files)
        {
            // Save the imageUploads to the database
            foreach (var imageUpload in imageUploads)
            {
                await AddAsync(imageUpload);
            }
            // Upload the images to blob storage
            await _blobStorageHandler.UploadImagesAsync(files, "/vehicles");
        }

        public async Task<IEnumerable<ImageUpload>> GetAllAsync()
        {
            return await _context.ImageUploads.ToListAsync();
        }

        public async Task<ImageUpload> AddAsync(ImageUpload entity)
        {
            await _context.ImageUploads.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<ImageUpload> UpdateAsync(ImageUpload entity)
        {
            _context.ImageUploads.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public Task DeleteAsync(ImageUpload entity)
        {
            _context.ImageUploads.Remove(entity);
            return _context.SaveChangesAsync();
        }
    }
}