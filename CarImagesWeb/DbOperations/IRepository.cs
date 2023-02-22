using System.Collections.Generic;
using System.Threading.Tasks;
using CarImagesWeb.DbContext;
using Microsoft.EntityFrameworkCore;

namespace CarImagesWeb.DbOperations
{
    /// <summary>
    /// A base interface for all repositories.
    /// </summary>
    public interface IRepository<T>
    {
        /// Retrieves an entity by its unique identifier.
        Task<T> GetById(int id);

        /// Retrieves all entities in the repository.
        Task<List<T>> GetAllAsync();

        /// Adds a new entity to the repository.
        Task<T> AddAsync(T entity);

        /// Updates an existing entity in the repository.
        Task<T> UpdateAsync(T entity);

        /// Deletes an entity from the repository.
        Task DeleteAsync(T entity);
    }

    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly CarImagesDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(CarImagesDbContext context)
        {
            _context = context;
            _dbSet = context.GetDbSet<T>();
        }
        
        public async Task<T> GetById(int id)
        {
            return await _dbSet.SingleOrDefaultAsync(t => t.GetType().GetProperty("Id").GetValue(t).Equals(id));

        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            return _context.SaveChangesAsync();
        }
    }

}