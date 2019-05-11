using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GeekBurger.Products.Extension;
using GeekBurger.Products.Helper;
using GeekBurger.Products.Repository;
using GeekBurger.Products.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace GeekBurger.Products
{
    public class Startup
    {
        public static IConfiguration Configuration;
        public IHostingEnvironment HostingEnvironment;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var mvcCoreBuilder = services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Products", Version = "v1" });
            });

            services.AddAutoMapper();

            var databasePath = "%DATABASEPATH%";
            var connection = Configuration.GetConnectionString("sql")
                .Replace(databasePath, HostingEnvironment.ContentRootPath);

            services.AddEntityFrameworkSqlite()
                .AddDbContext<ProductsDbContext>(o => o.UseSqlite(connection));

            services.AddScoped<IProductsRepository, ProductsRepository>();
            services.AddScoped<IProductChangedEventRepository, ProductChangedEventRepository>();
            services.AddScoped<IStoreRepository, StoreRepository>();
            services.AddSingleton<IProductChangedService, ProductChangedService>();

            services.AddSingleton<ILogService, LogService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ProductsDbContext productsDbContext)
        {
            if (HostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            using (var serviceScope = app
                .ApplicationServices
                .GetService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ProductsDbContext>();
                context.Database.EnsureCreated();
            }

            productsDbContext.Seed();
        }
    }
}
