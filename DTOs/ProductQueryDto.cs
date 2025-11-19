using System.ComponentModel.DataAnnotations;

namespace MyApi.DTOs
{
    public class ProductQueryDto
    {
        [StringLength(50, MinimumLength = 2)]
        public string? Name { get; set; }

        [Range(1, 100000)]
        public decimal? MinPrice { get; set; }

        [Range(1, 100000)]
        public decimal? MaxPrice { get; set; }

        [Range(1, 10)]
        public int? CategoryId { get; set; }
    }
}
