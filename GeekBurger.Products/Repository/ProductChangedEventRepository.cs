using System;
using System.Linq;
using GeekBurger.Products.Model;
using Microsoft.EntityFrameworkCore;

namespace GeekBurger.Products.Repository
{
    public class ProductChangedEventRepository : IProductChangedEventRepository
    {
        private readonly ProductsDbContext _dbContext;

        public ProductChangedEventRepository(ProductsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ProductChangedEvent Get(Guid eventId)
        {
            return _dbContext.ProductChangedEvents
                .FirstOrDefault(product => product.EventId == eventId);
        }

        public bool Add(ProductChangedEvent productChangedEvent)
        {
            productChangedEvent.Product =
                _dbContext.Products
                .FirstOrDefault(_ => _.ProductId == productChangedEvent.Product.ProductId);

            productChangedEvent.EventId = Guid.NewGuid();

            _dbContext.ProductChangedEvents.Add(productChangedEvent);

            return true;
        }

        public bool Update(ProductChangedEvent productChangedEvent)
        {
            return true;
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}