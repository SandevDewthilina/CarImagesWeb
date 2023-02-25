using System.Linq;
using CarImagesWeb.DbContext;
using Microsoft.EntityFrameworkCore;

namespace CarImagesWeb.DbOperations
{
    public interface IMappingRepository<T, T1, T2> 
    {
        
    }

    public class MappingRepository<T, T1, T2> : IMappingRepository<T, T1, T2> where T1 : class where T2 : class where T : class
    {
        private readonly CarImagesDbContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly IQueryable<T> _queryable;


        public MappingRepository(CarImagesDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
            _queryable = _dbSet.AsQueryable();
            _queryable = Repository<T>.BuildQueryable(_queryable, Repository<T>
                .GetComplexProperties().Select(p => p.Name));
        }
        
        
        
    }
}