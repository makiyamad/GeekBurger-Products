using GeekBurger.Products.Contract;
using GeekBurger.Products.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace GeekBurger.Products.Repository
{
    public static class ProductsContextExtensions
    {
        public static void Seed(this ProductsContext context)
        {

            context.Items.RemoveRange(context.Items);
            context.Products.RemoveRange(context.Products);
            context.Stores.RemoveRange(context.Stores);

            context.SaveChanges();

            context.Stores.AddRange(new List<Store> {
                new Store { Name = "Los Angeles - Pasadena", StoreId = new Guid("8048e9ec-80fe-4bad-bc2a-e4f4a75c834e") },
                new Store { Name = "Los Angeles - Beverly Hills", StoreId = new Guid("8d618778-85d7-411e-878b-846a8eef30c0") }
            });

            var productsTxt = File.ReadAllText("products.json");
            var products = JsonConvert.DeserializeObject<List<Product>>(productsTxt);
            context.Products.AddRange(products);

            context.SaveChanges();
        }
    }
}
