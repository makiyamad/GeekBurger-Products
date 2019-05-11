using System.Collections.Generic;

namespace GeekBurger.Products.Contract
{
    public class ProductChangedMessage
    {
        public ProductState State { get; set; }
        public ProductToGet Product { get; set; }
    }
}
