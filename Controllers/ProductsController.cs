using Microsoft.AspNetCore.Mvc;
using MyApi.Models;
using MyApi.Middleware; // Importăm pipe-ul
using System.Globalization;

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

        // 🔹 GET /product/list
        [HttpGet("list")]
        public ActionResult<IEnumerable<Product>> GetList()
        {
            // Transformăm toate numele în majuscule folosind pipe-ul
            var upperProducts = Products
                .Select(p => new Product
                {
                    Id = p.Id,
                    Name = p.Name.ToUpperCase(),
                    Price = p.Price
                })
                .ToList();

            return Ok(upperProducts);
        }

        // 🔹 GET /product/search?name=Mouse
        [HttpGet("search")]
        public ActionResult<IEnumerable<Product>> SearchByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest("Introdu un nume pentru căutare.");

            var searchName = name.ToUpperCase();

            // Căutare fără diferență între litere mari/mici
            var results = Products
                .Where(p => p.Name.ToUpperCase().Contains(searchName))
                .Select(p => new Product
                {
                    Id = p.Id,
                    Name = p.Name.ToUpperCase(),
                    Price = p.Price
                })
                .ToList();

            if (!results.Any())
                return NotFound("Niciun produs găsit cu acest nume.");

            return Ok(results);
        }
    }
}
