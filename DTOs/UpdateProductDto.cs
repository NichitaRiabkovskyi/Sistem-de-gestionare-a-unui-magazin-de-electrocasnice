using System.ComponentModel.DataAnnotations;

namespace MyApi.DTOs
{
    public class UpdateProductDto
    {
        [StringLength(50, MinimumLength = 3)]
        public string? Name { get; set; }

        [Range(1, 100000)]
        public decimal? Price { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        [Range(1, 10)]
        public int? CategoryId { get; set; }
    }
}
