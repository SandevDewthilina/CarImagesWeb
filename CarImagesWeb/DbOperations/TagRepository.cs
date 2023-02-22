using CarImagesWeb.DbContext;
using CarImagesWeb.Models;

namespace CarImagesWeb.DbOperations
{
    public interface ITagRepository : IRepository<Tag>
    {
        
    }
    
    public class TagRepository :  Repository<Tag>, ITagRepository
    {
        private readonly CarImagesDbContext _context;

        public TagRepository(CarImagesDbContext context) : base(context)
        {
            _context = context;
        }
    }
}