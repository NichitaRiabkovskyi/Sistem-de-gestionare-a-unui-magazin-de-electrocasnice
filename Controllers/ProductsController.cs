using Microsoft.AspNetCore.Mvc;
using MyApi.Models;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private static readonly List<Product> Products = new()
        {
            new Product { Id = 1, Name = "Mouse", Price = 50 },
            new Product { Id = 2, Name = "Keyboard", Price = 100 },
            new Product { Id = 3, Name = "Monitor", Price = 1200 },
            new Product { Id = 4, Name = "Laptop", Price = 3200 },
            new Product { Id = 5, Name = "Tablet", Price = 900 },
            new Product { Id = 6, Name = "Phone", Price = 1800 },
            new Product { Id = 7, Name = "SSD", Price = 400 },
            new Product { Id = 8, Name = "HDD", Price = 250 },
            new Product { Id = 9, Name = "RAM", Price = 300 },
            new Product { Id = 10, Name = "GPU", Price = 2500 },
        };

        // GET /product/list
        [HttpGet("list")]
        public ActionResult<IEnumerable<Product>> GetList()
        {
            return Ok(Products);
        }
    }
}
