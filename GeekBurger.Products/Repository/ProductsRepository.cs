using System;
using System.Collections.Generic;
using System.Linq;
using GeekBurger.Products.Contract;
using GeekBurger.Products.Helper;
using GeekBurger.Products.Model;
using GeekBurger.Products.Service;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GeekBurger.Products.Repository
{

    public class ProductsRepository : IProductsRepository
    {
        private ProductsContext _context;
        private IProductChangedService _productChangedService;

        public ProductsRepository(ProductsContext context, IProductChangedService productChangedService)
        {
            _context = context;
            _productChangedService = productChangedService;
        }

        public Product GetProductById(Guid productId)
        {
            return _context.Products?
                .Include(product => product.Items)
                .FirstOrDefault(product => product.ProductId == productId);
        }

        public List<Item> GetFullListOfItems()
        {
            return _context.Items.ToList();
        }

        public bool Add(Product product)
        {
            product.ProductId = Guid.NewGuid();
            _context.Products.Add(product);
            return true;
        }

        public bool Update(Product product)
        {
            return true;
        }

        public IEnumerable<Product> GetProductsByStoreName(string storeName)
        {
            var products = _context.Products?
                .Where(product =>
                    product.Store.Name.Equals(storeName,
                    StringComparison.InvariantCultureIgnoreCase))
                .Include(product => product.Items);

            return products;
        }

        public void Delete(Product product)
        {
            _context.Products.Remove(product);
        }

        public void Save()
        {
            _productChangedService
                .AddToMessageList(_context.ChangeTracker.Entries<Product>());

            _context.SaveChanges();

            _productChangedService.SendMessagesAsync();
        }
    }

    public class ProductChanged
    {
        public EntityState State { get; set; }
        public Product Product { get; set; }
    }
}
