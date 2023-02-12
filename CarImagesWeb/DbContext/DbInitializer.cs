using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarImagesWeb.DbContext
{
    public class DbInitializer
    {
        public static void Initialize(CarImagesDbContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}
