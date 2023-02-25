using CarImagesWeb.DbContext;
using CarImagesWeb.Models;

namespace CarImagesWeb.DbOperations
{
    public interface IRoleTagRepository : IRepository<UserRoleTagMapping>
    {
    }
    
    public class RoleTagRepository : Repository<UserRoleTagMapping>, IRoleTagRepository
    {
        public RoleTagRepository(CarImagesDbContext context) : base(context)
        {
            
        }
    }


}