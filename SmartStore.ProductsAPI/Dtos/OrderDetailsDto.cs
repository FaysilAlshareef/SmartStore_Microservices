using System.ComponentModel.DataAnnotations;

namespace SmartStore.ProductsAPI.Dto
{
    public class OrderDetailsDto
    {
        [Key]
        public int OrderDetailId { get; set; }

        public int OrderHeaderId { get; set; }

        public OrderHeaderDto OrderHeader { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }

        public int Count { get; set; }
    }
}
