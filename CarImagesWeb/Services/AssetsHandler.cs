using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CarImagesWeb.Controllers;
using CarImagesWeb.DbOperations;
using CarImagesWeb.DTOs;
using CarImagesWeb.Helpers;
using CarImagesWeb.Models;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;

namespace CarImagesWeb.Services
{
    public interface IAssetsHandler
    {
        /// <summary>
        ///     Get all the vehicle type assets from the database.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Asset>> GetVehiclesAsync();

        /// <summary>
        ///     Get all the container type assets from the database.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Asset>> GetContainersAsync();

        /// <summary>
        ///     Search the database for the asset from the image upload dto.
        /// </summary>
        /// <param name="imageUploadDto">
        ///     The image upload dto contains the asset type and the asset id.
        /// </param>
        /// <returns></returns>
        Task<Asset> GetAssetToUpload(ImageUploadDto imageUploadDto);
        
        Task ResetAssetsAsync(IFormFile sheet, Action<Exception> errorHandleCallback);
        Task DeleteAssetsAsync(IFormFile fileInput, Action<Exception> errorHandleCallback);
        Task<List<Asset>> GetAllAssets();
    }

    public class AssetsHandler : IAssetsHandler
    {
        private readonly IAssetRepository _repository;

        public AssetsHandler(IAssetRepository repository)
        {
            _repository = repository;
        }
        
        public async Task<IEnumerable<Asset>> GetVehiclesAsync()
        {
            return await _repository.GetAllAsync(a => a.Type == AssetType.Vehicle.ToString());
        }
        
        public async Task<IEnumerable<Asset>> GetContainersAsync()
        {
            return await _repository.GetAllAsync(a => a.Type == AssetType.Container.ToString());
        }

        public async Task<Asset> GetAssetToUpload(ImageUploadDto imageUploadDto)
        {
            var assetId = Enum.Parse<AssetType>(imageUploadDto.ImageCategory) switch
            {
                AssetType.Vehicle => int.Parse(imageUploadDto.VehicleId),
                AssetType.Container => int.Parse(imageUploadDto.ContainerId),
                _ => throw new NotImplementedException()
            };

            var asset = await _repository.GetAsync(a => a.Id == assetId);
            return asset;
        }

        public async Task ResetAssetsAsync(IFormFile fileInput, Action<Exception> errorHandleCallback)
        {
            // async callback function to delete all the assets from the database and add the new assets
            async Task DbOperation(List<Asset> assets)
            {
                //find existing assets by name and code
                var existingAssets = await _repository.FindAsync(
                        a => assets.Select(asset => asset.Name).Contains(a.Name) 
                             && assets.Select(asset => asset.Code).Contains(a.Code));
                
                if(existingAssets.Any())
                    await _repository.UpdateRangeAsync(existingAssets);
                
                //add the rest of the assets
                var newAssets = assets.Where(
                    a => !existingAssets.Select(asset => asset.Id).Contains(a.Id)).ToList();
                
                if(newAssets.Any())
                    await _repository.AddRangeAsync(newAssets);
                
                //delete the assets that are not in the new list
                var assetsToDelete = existingAssets.Where(
                    a => !assets.Select(asset => asset.Id).Contains(a.Id)).ToList();
                
                if(assetsToDelete.Any())
                    await _repository.DeleteRangeAsync(assetsToDelete);
            }
            
            await ManageAssetsAsync(fileInput, errorHandleCallback, DbOperation);
        }
        
        public async Task DeleteAssetsAsync(IFormFile fileInput, Action<Exception> errorHandleCallback)
        {
            // async callback function to delete the assets from the database
            async Task DbOperation(List<Asset> assets)
            {
                // Get the assets from the database by name and code
                var existingAssets = await _repository.FindAsync(
                    a => assets.Select(asset => asset.Name).Contains(a.Name) 
                         && assets.Select(asset => asset.Code).Contains(a.Code));
                
                await _repository.DeleteRangeAsync(existingAssets);
            }
            
            await ManageAssetsAsync(fileInput, errorHandleCallback, DbOperation);
        }

        public async Task<List<Asset>> GetAllAssets()
        {
            return await _repository.GetAllAsync();
        }

        private static async Task ManageAssetsAsync(IFormFile fileInput, Action<Exception> errorHandleCallback, 
            Func<List<Asset>, Task> dbOperation)
        {
            // Get the file extension
            var fileExtension = Path.GetExtension(fileInput.FileName);

            switch (fileExtension)
            {
                case ".csv":
                {
                    using var reader = new StreamReader(fileInput.OpenReadStream());
                    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                    var assets = new List<Asset>();
                    // Read the CSV data into a list of Asset objects
                    try
                    {
                        var assetRecords = csv.GetRecords<AssetRecord>().ToList();
                        
                        if(assetRecords.Count == 0) errorHandleCallback(
                            new Exception("No data found in the file"));
                        
                        assets.AddRange(assetRecords.Select(assetRecord => new Asset
                        {
                            Name = assetRecord.Name,
                            Code = assetRecord.Code,
                            Type = assetRecord.Type
                        }));
                    }
                    catch (HeaderValidationException e)
                    {

                        var headerNames = new List<string>();
                        // get the missing header names
                        foreach (var invalidHeader in e.InvalidHeaders)
                        {
                            // if header name is not already in the list
                            if (!headerNames.Contains(invalidHeader.Names[0]))
                                headerNames.AddRange(invalidHeader.Names);
                        }
                        
                        // Handle the missing field exception
                        var exception = new Exception(
                            "The following headers are missing: " + headerNames.Join(", "));
                        
                        errorHandleCallback(exception);
                    }
                    
                    // Perform the database operation
                    if (assets.Any())
                        await dbOperation(assets);
                    
                    break;
                }
            }
        }

        
    }
}