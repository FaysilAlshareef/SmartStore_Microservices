using SmartStore.ProductsAPI.Entities;
using System.ComponentModel.DataAnnotations;

namespace SmartStore.ProductsAPI.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Range(1, double.MaxValue)]
        public decimal Price { get; set; }
        public string? PictureUrl { get; set; }
        [Required]
        [Range(1, double.MaxValue)]
        public decimal Quantity { get; set; }
        [Required]
        public string Category { get; set; }

    }
}
