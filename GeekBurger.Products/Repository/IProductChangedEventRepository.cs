using System;
using GeekBurger.Products.Model;

namespace GeekBurger.Products.Repository
{
    public interface IProductChangedEventRepository
    {
        ProductChangedEvent Get(Guid eventId);
        bool Add(ProductChangedEvent productChangedEvent);
        bool Update(ProductChangedEvent productChangedEvent);
        void Save();
    }
}