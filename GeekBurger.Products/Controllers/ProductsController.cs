using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GeekBurger.Products.Contract;
using GeekBurger.Products.Repository;
using Microsoft.AspNetCore.Mvc;

namespace GeekBurger.Products.Controllers
{

    [Route("api/products")]
    public class ProductsController : Controller
    {
        private IProductsRepository _productsRepository;
        private IMapper _mapper;

        public ProductsController(IProductsRepository productsRepository, IMapper mapper)
        {
            _productsRepository = productsRepository;
            _mapper = mapper;
        }

        [HttpGet()]
        public IActionResult GetProductsByStoreName([FromQuery] string storeName)
        {
            var productsByStore = _productsRepository.GetProductsByStoreName(storeName).ToList();

            if (productsByStore.Count <= 0)
                return NotFound();

            var productsToGet = _mapper.Map<IEnumerable<ProductToGet>>(productsByStore);

            return Ok(productsToGet);
        }
    }
}