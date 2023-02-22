using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CarImagesWeb.Models;
using CarImagesWeb.Services;
using Microsoft.AspNetCore.Http;

namespace CarImagesWeb.DbContext
{
    public interface IAssetsRepository
    {
        Task SaveAssetAsync(Asset asset, IFormFileCollection files);
    }
    
    public class AssetsRepository : IAssetsRepository
    {
        private readonly CarImagesDbContext _context;
        private readonly IBlobStorageHandler _blobStorageHandler;

        public AssetsRepository(CarImagesDbContext context, IBlobStorageHandler blobStorageHandler)
        {
            _context = context;
            _blobStorageHandler = blobStorageHandler;
        }

        public async Task SaveAssetAsync(Asset asset, IFormFileCollection files)
        {
            // Save the asset to the database
            // _context.Assets.Add(asset);
            // _context.SaveChanges();
            
            // Upload the images to blob storage
            await _blobStorageHandler.UploadImagesAsync(files, "/vehicles");
        }
    }
}