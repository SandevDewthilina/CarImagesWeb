using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CarImagesWeb.DbContext;
using CarImagesWeb.DbOperations;
using CarImagesWeb.Models;
using CarImagesWeb.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace CarImagesWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            
            // MySQL DB connection service
            services.AddDbContextPool<CarImagesDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            // EFCore identity and set password validations  
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // options.Password.RequiredLength = 6;
                // options.Password.RequireDigit = false;
                // options.Password.RequireUppercase = false;
                // options.Password.RequiredUniqueChars = 0;
                // options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<CarImagesDbContext>();

            // Add repository services
            services.AddScoped<IAssetRepository, AssetRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<IImagesRepository, ImagesRepository>();
            // Add handler services
            services.AddScoped<IAssetsHandler, AssetsHandler>(); 
            services.AddScoped<ITagsHandler, TagsHandler>();
            services.AddScoped<ICountryHandler, CountryHandler>();
            services.AddScoped<IImagesHandler, ImagesHandler>();
            services.AddScoped<IBlobStorageHandler, BlobStorageHandler>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "node_modules")),

                RequestPath = "/node_modules"
            });

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Images}/{action=Upload}/{id?}");
            });
        }
    }
}