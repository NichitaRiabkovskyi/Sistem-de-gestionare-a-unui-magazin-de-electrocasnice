using Microsoft.AspNetCore.Mvc;
using MyApi.Data;
using MyApi.Models;
using System.Linq;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("admin")]
    public class AdminController : ControllerBase
    {
        // Nivel 9: /admin/reports -> doar admin (middleware)
        [HttpGet("reports")]
        public ActionResult GetReports()
        {
            // exemplu: numar produse
            return Ok(new { totalProducts = InMemoryData.Products.Count });
        }

        // Nivel 9: /admin/edit/{id} -> editare produs (PUT)
        [HttpPut("edit/{id:int}")]
        public ActionResult EditProduct(int id, [FromBody] Product updated)
        {
            var p = InMemoryData.Products.FirstOrDefault(x => x.Id == id);
            if (p == null) return NotFound();
            p.Name = updated.Name;
            p.Price = updated.Price;
            p.Description = updated.Description;
            return Ok(p);
        }
    }
}
