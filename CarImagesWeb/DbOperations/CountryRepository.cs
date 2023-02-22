using CarImagesWeb.DbContext;
using CarImagesWeb.Models;

namespace CarImagesWeb.DbOperations
{
    public interface ICountryRepository
    {
        
    }
    
    public class CountryRepository : Repository<Country>, ICountryRepository
    {
        private readonly CarImagesDbContext _context;

        public CountryRepository(CarImagesDbContext context) : base(context)
        {
            _context = context;
        }
    }
}