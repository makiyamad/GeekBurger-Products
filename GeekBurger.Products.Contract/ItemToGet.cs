using System;
using System.ComponentModel.DataAnnotations;

namespace GeekBurger.Products.Contract
{
    public class ItemToGet
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; }
    }
}
