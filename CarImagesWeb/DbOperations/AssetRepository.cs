using CarImagesWeb.DbContext;
using CarImagesWeb.Models;

namespace CarImagesWeb.DbOperations
{
    public interface IAssetRepository : IRepository<Asset>
    {
    }

    public class AssetRepository : Repository<Asset>, IAssetRepository
    {
        private readonly CarImagesDbContext _context;

        public AssetRepository(CarImagesDbContext context) : base(context)
        {
            _context = context;
        }
    }
}