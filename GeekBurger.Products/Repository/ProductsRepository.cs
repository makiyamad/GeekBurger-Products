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
        private ProductsDbContext _dbContext;
        private IProductChangedService _productChangedService;

        public ProductsRepository(ProductsDbContext dbContext, IProductChangedService productChangedService)
        {
            _dbContext = dbContext;
            _productChangedService = productChangedService;
        }

        public Product GetProductById(Guid productId)
        {
            return _dbContext.Products?
                .Include(product => product.Items)
                .FirstOrDefault(product => product.ProductId == productId);
        }

        public List<Item> GetFullListOfItems()
        {
            return _dbContext.Items.ToList();
        }

        public bool Add(Product product)
        {
            product.ProductId = Guid.NewGuid();
            _dbContext.Products.Add(product);
            return true;
        }

        public bool Update(Product product)
        {
            return true;
        }

        public IEnumerable<Product> GetProductsByStoreName(string storeName)
        {
            var products = _dbContext.Products?
                .Where(product =>
                    product.Store.Name.Equals(storeName,
                    StringComparison.InvariantCultureIgnoreCase))
                .Include(product => product.Items);

            return products;
        }

        public void Delete(Product product)
        {
            _dbContext.Products.Remove(product);
        }

        public void Save()
        {
            _productChangedService
                .AddToMessageList(_dbContext.ChangeTracker.Entries<Product>());

            _dbContext.SaveChanges();

            _productChangedService.SendMessagesAsync();
        }
    }
}
