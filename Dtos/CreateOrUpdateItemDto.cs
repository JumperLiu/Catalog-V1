using System.ComponentModel.DataAnnotations;

namespace Catalog.Dtos
{
    public record CreateOrUpdateItemDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string Name { get; init; }
        [Required]
        [Range(0.5, 1000)]
        public decimal Price { get; init; }
    }
}