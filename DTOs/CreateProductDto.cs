using System.ComponentModel.DataAnnotations;

namespace MyApi.DTOs
{
    public class CreateProductDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; } = "";

        [Required]
        [Range(1, 100000)]
        public decimal Price { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        [Required]
        [Range(1, 10)]
        public int CategoryId { get; set; }

        // Validare condițională: dacă prețul > 5000, descrierea devine obligatorie
        public bool ShouldRequireDescription() => Price > 5000 && string.IsNullOrWhiteSpace(Description);
    }
}
