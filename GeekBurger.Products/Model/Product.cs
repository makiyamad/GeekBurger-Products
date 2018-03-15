using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeekBurger.Products.Model
{
    public class Product
    {
        [ForeignKey("StoreId")]
        public Store Store { get; set; }
        public Guid StoreId { get; set; }
        [Key]
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public ICollection<Item> Items { get; set; }
            = new List<Item>();
        public decimal Price { get; set; }
    }
}
