using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GeekBurger.Products.Model;
using GeekBurger.Products.Repository;
using Newtonsoft.Json;

namespace GeekBurger.Products.Extension
{
    public static class ProductsContextExtensions
    {
        public static void Seed(this ProductsDbContext dbContext)
        {

            if (dbContext.Items.Any() ||
                dbContext.Products.Any() ||
                dbContext.Stores.Any())
                return;

            dbContext.Stores.AddRange(new List<Store> {
                new Store { Name = "Los Angeles - Pasadena", StoreId = new Guid("8048e9ec-80fe-4bad-bc2a-e4f4a75c834e") },
                new Store { Name = "Los Angeles - Beverly Hills", StoreId = new Guid("8d618778-85d7-411e-878b-846a8eef30c0") }
            });

            var productsTxt = File.ReadAllText("products.json");
            var products = JsonConvert.DeserializeObject<List<Product>>(productsTxt);
            dbContext.Products.AddRange(products);

            dbContext.SaveChanges();
        }
    }
}
