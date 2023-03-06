using System.Threading.Tasks;
using CarImagesWeb.DbContext;
using CarImagesWeb.Models;

namespace CarImagesWeb.DbOperations
{
    
    public interface IVehicleContainerRepository: IRepository<ContainerVehicleMapping>
    {
        
    }
    public class VehicleContainerRepository : Repository<ContainerVehicleMapping>, IVehicleContainerRepository
    {
        private readonly CarImagesDbContext _context;
        
        public VehicleContainerRepository(CarImagesDbContext context) : base(context)
        {
            _context = context;
        }
    }
}