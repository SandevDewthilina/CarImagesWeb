using System.Collections.Generic;
using System.Threading.Tasks;
using CarImagesWeb.DbOperations;
using CarImagesWeb.Helpers;
using CarImagesWeb.Models;

namespace CarImagesWeb.Services
{
    public interface IAssetsHandler
    {
        Task<IEnumerable<Asset>> GetVehiclesAsync();
        Task<IEnumerable<Asset>> GetContainersAsync();
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
    }
}