using CarImagesWeb.DbContext;
using Microsoft.EntityFrameworkCore;

namespace CarImagesWeb.DbOperations
{
    public interface IMappingRepository<T1, T2>
    {
        
    }
    
    public class MappingRepository<T1, T2> : IMappingRepository<T1, T2> where T1 : class where T2 : class
    {
        private readonly CarImagesDbContext _context;
        private readonly DbSet<T1> _dbSet1;
        private readonly DbSet<T2> _dbSet2;

        public MappingRepository(CarImagesDbContext context)
        {
            _context = context;
            _dbSet1 = context.GetDbSet<T1>();
            _dbSet2 = context.GetDbSet<T2>();
        }
    }
}