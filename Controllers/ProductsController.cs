using Microsoft.AspNetCore.Mvc;
using MyApi.Models;
using MyApi.DTOs;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Text;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        // 🔹 listă in-memory de produse
        private static readonly List<Product> Products = new()
        {
            new Product { Id = 1, Name = "Mouse",    Price = 50,   Description = "Mouse wireless",      CategoryId = 1 },
            new Product { Id = 2, Name = "Keyboard", Price = 100,  Description = "Tastatură mecanică",  CategoryId = 1 },
            new Product { Id = 3, Name = "Monitor",  Price = 1200, Description = "Monitor 27 inch",     CategoryId = 2 },
            new Product { Id = 4, Name = "Laptop",   Price = 3200, Description = "Laptop performant",   CategoryId = 3 },
            new Product { Id = 5, Name = "Tablet",   Price = 900,  Description = "Tabletă 10 inch",     CategoryId = 3 },
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
        public IActionResult GetById([FromRoute][Range(1, int.MaxValue)] int id)
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
        public IActionResult Delete([FromRoute][Range(1, int.MaxValue)] int id)
        {
            var product = Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            Products.Remove(product);
            return Ok(new { deleted = true });
        }

        // 🔹 POST /product/import  –– LAB 4: Import CSV cu validare
        [HttpPost("import")]
        public async Task<IActionResult> ImportCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Fișierul este obligatoriu.");
            }

            // ✅ Validare fișier (extensie .csv)
            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Fișierul trebuie să fie de tip .csv.");
            }

            // ✅ Dimensiune maximă 5MB
            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest("Fișierul este prea mare (max 5MB).");
            }

            var errors = new List<object>();
            var importedProducts = new List<Product>();

            int totalRows = 0;
            int successful = 0;
            int failed = 0;

            using (var stream = file.OpenReadStream())
            using (var reader = new StreamReader(stream))
            {
                // citim header-ul
                var headerLine = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(headerLine))
                {
                    return BadRequest("Fișierul nu conține date.");
                }

                var headerColumns = headerLine.Split(',', StringSplitOptions.TrimEntries);

                // ne așteptăm la: Name,Price,Description,CategoryId
                var expectedColumns = new[] { "Name", "Price", "Description", "CategoryId" };

                if (headerColumns.Length != expectedColumns.Length ||
                    !expectedColumns.All(c => headerColumns.Contains(c)))
                {
                    return BadRequest($"Header-ul CSV nu corespunde. Aștept coloanele: {string.Join(", ", expectedColumns)}");
                }

                int rowNumber = 1; // header = rândul 1

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    rowNumber++;

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    totalRows++;

                    var columns = line.Split(',', StringSplitOptions.TrimEntries);

                    if (columns.Length != expectedColumns.Length)
                    {
                        failed++;
                        errors.Add(new
                        {
                            row = rowNumber,
                            data = line,
                            errors = new[] { "Număr de coloane invalid." }
                        });
                        continue;
                    }

                    var dto = new CreateProductDto();

                    dto.Name = columns[Array.IndexOf(headerColumns, "Name")];

                    if (!decimal.TryParse(
                            columns[Array.IndexOf(headerColumns, "Price")],
                            NumberStyles.Any,
                            CultureInfo.InvariantCulture,
                            out var price))
                    {
                        dto.Price = 0;
                    }
                    else
                    {
                        dto.Price = price;
                    }

                    var descriptionValue = columns[Array.IndexOf(headerColumns, "Description")];
                    dto.Description = string.IsNullOrWhiteSpace(descriptionValue) ? null : descriptionValue;

                    if (!int.TryParse(columns[Array.IndexOf(headerColumns, "CategoryId")], out var categoryId))
                    {
                        dto.CategoryId = 0;
                    }
                    else
                    {
                        dto.CategoryId = categoryId;
                    }

                    // ✅ Validare DataAnnotations
                    var validationResults = new List<ValidationResult>();
                    var validationContext = new ValidationContext(dto);

                    bool isValid = Validator.TryValidateObject(
                        dto, validationContext, validationResults, validateAllProperties: true);

                    var errorMessages = validationResults
                        .Select(v => v.ErrorMessage!)
                        .ToList();

                    // ✅ Validare condițională: dacă prețul > 5000 => descriere obligatorie
                    if (dto.ShouldRequireDescription())
                    {
                        errorMessages.Add("Descrierea este obligatorie când prețul este mai mare de 5000.");
                        isValid = false;
                    }

                    if (!isValid)
                    {
                        failed++;
                        errors.Add(new
                        {
                            row = rowNumber,
                            data = new
                            {
                                dto.Name,
                                dto.Price,
                                dto.Description,
                                dto.CategoryId
                            },
                            errors = errorMessages
                        });
                        continue;
                    }

                    var product = new Product
                    {
                        Id = _nextId++,
                        Name = dto.Name,
                        Price = dto.Price,
                        Description = dto.Description ?? "",
                        CategoryId = dto.CategoryId
                    };

                    Products.Add(product);
                    importedProducts.Add(product);
                    successful++;
                }
            }

            var response = new
            {
                totalRows,
                successful,
                failed,
                errors,
                imported = importedProducts.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Description,
                    p.CategoryId
                })
            };

            return Ok(response);
        }

        // 🔹 GET /product/export –– LAB 4: Export CSV cu filtrare prin query params
        [HttpGet("export")]
        public IActionResult ExportToCsv([FromQuery] ProductQueryDto query)
        {
            // 1. Filtrare pe baza DTO-ului de query
            IEnumerable<Product> filtered = Products;

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                filtered = filtered.Where(p =>
                    p.Name.Contains(query.Name, StringComparison.OrdinalIgnoreCase));
            }

            if (query.CategoryId.HasValue)
            {
                filtered = filtered.Where(p => p.CategoryId == query.CategoryId.Value);
            }

            if (query.MinPrice.HasValue)
            {
                filtered = filtered.Where(p => p.Price >= query.MinPrice.Value);
            }

            if (query.MaxPrice.HasValue)
            {
                filtered = filtered.Where(p => p.Price <= query.MaxPrice.Value);
            }

            var list = filtered.ToList();
            if (!list.Any())
            {
                return NotFound("Nu există produse pentru filtrul specificat.");
            }

            // 2. Generăm CSV: include TOATE câmpurile entității (Id, Name, Price, Description, CategoryId)
            var sb = new StringBuilder();
            sb.AppendLine("Id,Name,Price,Description,CategoryId");

            foreach (var p in list)
            {
                var name = (p.Name ?? "").Replace("\"", "\"\"");
                var description = (p.Description ?? "").Replace("\"", "\"\"");

                sb.AppendLine(
                    $"{p.Id}," +
                    $"\"{name}\"," +
                    $"{p.Price.ToString(CultureInfo.InvariantCulture)}," +
                    $"\"{description}\"," +
                    $"{p.CategoryId}"
                );
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            var fileName = $"products_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            // 3. Headers corecte pentru download
            return File(bytes, "text/csv", fileName);
        }
    }
}
