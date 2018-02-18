using AutoMapper;
using GeekBurger.Products.Contract;
using GeekBurger.Products.Model;
using GeekBurger.Products.Repository;
using System;
using System.Linq;

namespace GeekBurger.Products
{
public class MatchItemsFromRepository : IMappingAction<ItemToUpsert, Item>
{
    private IProductsRepository _productRepository;
    public MatchItemsFromRepository(IProductsRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public void Process(ItemToUpsert source, Item destination)
    {
        var fullListOfItems = _productRepository.GetFullListOfItems();

        var itemFound = fullListOfItems?
                .FirstOrDefault(item => source.Name
                .Equals(destination.Name, 
                    StringComparison.InvariantCultureIgnoreCase));

        if (itemFound == null)
            destination.ItemId = Guid.NewGuid();
        else
            destination = itemFound;
    }
}
}