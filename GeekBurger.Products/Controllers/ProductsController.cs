using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeekBurger.Products.Contract;
using Microsoft.AspNetCore.Mvc;

namespace GeekBurger.Products.Controllers
{
    [Route("api/products")]
    public class ProductsController : Controller
    {
        private List<Product> Products;

        public ProductsController()
        {
            var losAngelesStore = new Guid("8048e9ec-80fe-4bad-bc2a-e4f4a75c834e");
            var beverlyHillsStore = new Guid("8d618778-85d7-411e-878b-846a8eef30c0");

            var beef = Guid.NewGuid();
            var mustard = Guid.NewGuid();
            var bread = Guid.NewGuid();

            Products =
                new List<Product>()
                {
            new Product { ProductId = Guid.NewGuid(), Name = "Darth Bacon",
                Image = "hamb1.png", StoreId = losAngelesStore,
                Items = new List<Item> {new Item { ItemId = beef, Name = "beef"},
                    new Item { ItemId = Guid.NewGuid(), Name = "ketchup" },
                    new Item { ItemId = bread, Name = "bread" } }
            },
            new Product { ProductId = Guid.NewGuid(), Name = "Cap. Spork",
                Image = "hamb2.png", StoreId = losAngelesStore,
                Items = new List<Item> {new Item { ItemId = Guid.NewGuid(), Name = "pork"},
                    new Item { ItemId = mustard, Name = "mustard" },
                    new Item { ItemId = Guid.NewGuid(), Name = "whole bread" } }
            },
            new Product { ProductId = Guid.NewGuid(), Name = "Beef Turner",
                Image = "hamb3.png", StoreId = beverlyHillsStore,
                Items = new List<Item> {new Item { ItemId = beef, Name = "beef"},
                    new Item { ItemId = mustard, Name = "mustard" },
                    new Item { ItemId = bread, Name = "bread" } }
            }
                };
        }

        [HttpGet("{storeid}")]
        public IActionResult GetProductsByStoreId(Guid storeId)
        {
            var productsByStore = Products.Where(product => product.StoreId == storeId).ToList();

            if (productsByStore.Count <= 0)
                return NotFound();

            return Ok(productsByStore);
        }
    }
}