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