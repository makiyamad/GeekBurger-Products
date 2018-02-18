using GeekBurger.Products.Contract;
using GeekBurger.Products.Model;
using System;

namespace GeekBurger.Products.Repository
{
    public interface IStoreRepository
    {
        Store GetStoreByName(string name);
    }
}
