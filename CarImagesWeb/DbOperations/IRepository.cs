using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using CarImagesWeb.DbContext;
using Microsoft.EntityFrameworkCore;

namespace CarImagesWeb.DbOperations
{
    /// <summary>
    ///     A base interface for all repositories.
    /// </summary>
    public interface IRepository<T>
    {
        /// Returns a single entity by a given predicate.
        Task<T> GetAsync(Expression<Func<T, bool>> predicate, IEnumerable<string> includes = null);

        /// Returns a multiple entities by a given predicate.
        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, IEnumerable<string> includes = null);

        /// Retrieves all entities in the repository.
        Task<List<T>> GetAllAsync(IEnumerable<string> includes = null);

        ///Retrieves all entities in the repository based on the predicate.
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate, IEnumerable<string> includes = null);

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
        private readonly IQueryable<T> queryable;

        protected Repository(CarImagesDbContext context)
        {
            _context = context;
            _dbSet = context.GetDbSet<T>();
            queryable = _dbSet.AsQueryable();
            queryable = BuildQueryable(queryable, GetComplexProperties().Select(p => p.Name));
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate, IEnumerable<string> includes = null)
        {
            return await queryable.SingleOrDefaultAsync(predicate);
        }

        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, IEnumerable<string> includes = null)
        {
            return await queryable.Where(predicate).ToListAsync();
        }

        public async Task<List<T>> GetAllAsync(IEnumerable<string> includes = null)
        {
            return await queryable.ToListAsync();
        }

        public Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate, IEnumerable<string> includes = null)
        {
            return queryable.Where(predicate).ToListAsync();
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

        private static IQueryable<T> BuildQueryable(IQueryable<T> queryable, IEnumerable<string> includes)
        {
            return includes == null
                ? queryable
                : includes.Aggregate(queryable, (current, include) => current.Include(include));
        }

        private static IEnumerable<PropertyInfo> GetComplexProperties()
        {
            // Get all the properties of T
            var properties = typeof(T).GetProperties();

            return (from property in properties
                let propertyType = property.PropertyType
                where propertyType.IsClass && propertyType != typeof(string)
                select property).ToList();
        }
    }
}