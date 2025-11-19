using Microsoft.AspNetCore.Mvc;
using MyApi.Models;
using MyApi.DTOs;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private static readonly List<Product> Products = new()
        {
            new Product { Id = 1, Name = "Mouse", Price = 50, Description = "Mouse wireless", CategoryId = 1 },
            new Product { Id = 2, Name = "Keyboard", Price = 100, Description = "Tastatură mecanică", CategoryId = 1 },
            new Product { Id = 3, Name = "Monitor", Price = 1200, Description = "Monitor 27 inch", CategoryId = 2 },
            new Product { Id = 4, Name = "Laptop", Price = 3200, Description = "Laptop performant", CategoryId = 3 },
            new Product { Id = 5, Name = "Tablet", Price = 900, Description = "Tabletă 10 inch", CategoryId = 3 },
        };

        private static int _nextId = 6;

        // 🔹 GET /product/list
        [HttpGet("list")]
        public ActionResult<IEnumerable<Product>> GetList()
        {
            return Ok(Products);
        }

        // 🔹 GET /product/search?name=Mouse
        [HttpGet("search")]
        public ActionResult<IEnumerable<Product>> SearchByName(
            [FromQuery][StringLength(50, MinimumLength = 2)] string name)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest("Introdu un nume pentru căutare.");

            var results = Products
                .Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!results.Any())
                return NotFound("Niciun produs găsit cu acest nume.");

            return Ok(results);
        }

        // 🔹 POST /product (create product)
        [HttpPost]
        public IActionResult Create([FromBody] CreateProductDto dto)
        {
            // validare condițională: dacă prețul > 5000, descrierea devine obligatorie
            if (dto.ShouldRequireDescription())
                ModelState.AddModelError("Description", "Descrierea este obligatorie pentru produse peste 5000 RON.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = new Product
            {
                Id = _nextId++,
                Name = dto.Name,
                Price = dto.Price,
                Description = dto.Description ?? "",
                CategoryId = dto.CategoryId
            };

            Products.Add(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        // 🔹 GET /product/{id}
        [HttpGet("{id:int}")]
        public IActionResult GetById([FromRoute][System.ComponentModel.DataAnnotations.Range(1, int.MaxValue)] int id)
        {
            var product = Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // 🔹 PUT /product/{id}
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] UpdateProductDto dto)
        {
            var product = Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.Name != null) product.Name = dto.Name;
            if (dto.Price.HasValue) product.Price = dto.Price.Value;
            if (dto.Description != null) product.Description = dto.Description;
            if (dto.CategoryId.HasValue) product.CategoryId = dto.CategoryId.Value;

            return Ok(product);
        }

        // 🔹 DELETE /product/{id}
        [HttpDelete("{id:int}")]
        public IActionResult Delete([FromRoute][System.ComponentModel.DataAnnotations.Range(1, int.MaxValue)] int id)
        {
            var product = Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            Products.Remove(product);
            return Ok(new { deleted = true });
        }
    }
}
