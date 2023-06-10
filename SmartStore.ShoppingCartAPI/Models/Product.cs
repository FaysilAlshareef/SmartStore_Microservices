using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartStore.ShoppingCartAPI.Models
{
    public class Product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Range(1, Double.MaxValue)]
        public double Price { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string PictureUrl { get; set; }
        public decimal Quantity { get; set; }
        [NotMapped]
        public List<CartDetails> CartDetails { get; set; }
    }
}
