using System;
using System.Collections.Generic;

namespace GeekBurger.Products.Contract
{
    public class Product
    {
        public Guid StoreId { get; set; }
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public List<Item> Items { get; set; }
    }
}
