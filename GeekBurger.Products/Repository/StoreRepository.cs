using GeekBurger.Products.Contract;
using GeekBurger.Products.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeekBurger.Products.Repository
{
    public class StoreRepository : IStoreRepository
    {
        private ProductsContext _context { get; set; }

        public StoreRepository(ProductsContext context)
        {
            _context = context;
        }


        public Store GetStoreByName(string name)
        {
            return _context.Stores.FirstOrDefault(store => store.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));   
        }
    }
}
