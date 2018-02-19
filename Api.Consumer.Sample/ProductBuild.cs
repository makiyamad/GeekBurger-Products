using System.Text;
using GeekBurger.Products.Contract;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Api.Consumer.Sample
{
    public static class ProductBuild
    {
        public static int i = 0;
        public static byte[] NewProduct() {
            var product = new ProductToUpsert()
            {
                StoreName = i++%2 == 0 ? "Los Angeles - Pasadena" : "Los Angeles - Beverly Hills",
                Image = "hamb1.png",
                Name = "Beef the Elegant",
                Items = new List<ItemToUpsert> {
                    new ItemToUpsert { Name = "bread" },
                    new ItemToUpsert { Name = "mustard" },
                    new ItemToUpsert { Name = "beef" }
                }
            };

            var productJson = JsonConvert.SerializeObject(product);
            return Encoding.UTF8.GetBytes(productJson);
        }
    }
}