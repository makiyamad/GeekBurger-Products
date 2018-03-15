using System;
using System.Collections.Generic;

namespace GeekBurger.Products.Contract
{
    public class ProductToUpsert
    {
        public string StoreName { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public List<ItemToUpsert> Items { get; set; }

        public decimal Price { get; set; }
    }
}
