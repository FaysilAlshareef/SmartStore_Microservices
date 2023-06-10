
using System.ComponentModel.DataAnnotations;

namespace SmartStore.UI.Dtos.Products
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
        public IFormFile Image { get; set; }
        [Required]
        [Range(1, double.MaxValue)]
        public decimal Quantity { get; set; }
        public decimal CartQuantity { get; set; }
        [Required]
        public string Category { get; set; }

        
    }
}
