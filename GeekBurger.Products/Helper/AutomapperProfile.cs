using AutoMapper;
using GeekBurger.Products.Contract;
using GeekBurger.Products.Model;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GeekBurger.Products
{
    public class AutomapperProfile :Profile
    {
        public AutomapperProfile()
        {
            CreateMap<ProductToUpsert, Product>().AfterMap<MatchStoreFromRepository>();
            CreateMap<Product, ProductToUpsert>();
            CreateMap<ItemToUpsert, Item>().AfterMap<MatchItemsFromRepository>();
            CreateMap<Product, ProductToGet>();
            CreateMap<Item, ItemToGet>();
            CreateMap<EntityEntry<Product>, ProductChangedMessage>()
            .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Entity));
            CreateMap<EntityEntry<Product>, ProductChangedEvent>()
            .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Entity));
        }
    }
}
