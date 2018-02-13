using System.Collections.Generic;

namespace GeekBurger.Products.Contract
{
    public class ProductChangedMessage
    {
        public ProductState State { get; set; }
        public ProductToGet Product { get; set; }
    }

    public enum ProductState
    {
        Deleted = 2,
        Modified = 3,
        Added = 4
    }
}
