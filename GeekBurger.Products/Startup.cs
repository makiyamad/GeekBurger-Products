using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GeekBurger.Products.Helper;
using GeekBurger.Products.Repository;
using GeekBurger.Products.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeekBurger.Products
{
    public class Startup
    {
        public static IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var mvcCoreBuilder = services.AddMvcCore();

            mvcCoreBuilder
                .AddFormatterMappings()
                .AddJsonFormatters()
                .AddCors();

            services.AddAutoMapper();

            services.AddDbContext<ProductsContext>(o => o.UseInMemoryDatabase("geekburger-products"));
            services.AddScoped<IProductsRepository, ProductsRepository>();
            services.AddScoped<IStoreRepository, StoreRepository>();
            services.AddSingleton<IProductChangedService, ProductChangedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ProductsContext productsContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            productsContext.Seed();
        }
    }
}
