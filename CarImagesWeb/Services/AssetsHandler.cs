using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarImagesWeb.DbOperations;
using CarImagesWeb.DTOs;
using CarImagesWeb.Helpers;
using CarImagesWeb.Models;

namespace CarImagesWeb.Services
{
    public interface IAssetsHandler
    {
        /// <summary>
        /// Get all the vehicle type assets from the database.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Asset>> GetVehiclesAsync();
        
        /// <summary>
        /// Get all the container type assets from the database.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Asset>> GetContainersAsync();
        
        /// <summary>
        /// Search the database for the asset from the image upload dto.
        /// </summary>
        /// <param name="imageUploadDto">
        /// The image upload dto contains the asset type and the asset id.
        /// </param>
        /// <returns></returns>
        Task<Asset> GetAssetToUpload(ImageUploadDto imageUploadDto);
    }
    
    public class AssetsHandler : IAssetsHandler
    {
        private readonly IAssetRepository _repository;

        public AssetsHandler(IAssetRepository repository)
        {
            _repository = repository;
        }
        
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Asset>> GetVehiclesAsync()
        {
            return await _repository.GetAllAsync(a => a.Type == AssetType.Vehicle.ToString());
        }
        
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Asset>> GetContainersAsync()
        {
            return await _repository.GetAllAsync(a => a.Type == AssetType.Container.ToString());
        }
        
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task<Asset> GetAssetToUpload(ImageUploadDto imageUploadDto)
        {
            var assetId = Enum.Parse<AssetType>(imageUploadDto.ImageCategory) switch
            {
                AssetType.Vehicle => int.Parse(imageUploadDto.VehicleId),
                AssetType.Container => int.Parse(imageUploadDto.ContainerId),
                _ => throw new NotImplementedException()
            };

            var asset = await _repository.GetById(assetId);
            return asset;
        }
    }
}