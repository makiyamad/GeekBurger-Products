using System.Collections.Generic;

namespace GeekBurger.Products.Contract
{
    public class ProductsChanged
    {
        public List<int> DeletedProducts { get; set; }
        public List<Product> UpdatedProducts { get; set; }
        public List<Product> InsertedProducts { get; set; }
    }
}
