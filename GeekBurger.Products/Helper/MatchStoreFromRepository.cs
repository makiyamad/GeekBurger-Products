using AutoMapper;
using GeekBurger.Products.Contract;
using GeekBurger.Products.Model;
using GeekBurger.Products.Repository;

namespace GeekBurger.Products
{
    public class MatchStoreFromRepository : IMappingAction<ProductToUpsert, Product>
    {
        private IStoreRepository _storeRepository;
        public MatchStoreFromRepository(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }

        public void Process(ProductToUpsert source, Product destination)
        {
            var store = _storeRepository.GetStoreByName(source.StoreName);

            if(store != null)
                destination.StoreId = store.StoreId;
        }
    }
}