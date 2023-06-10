namespace SmartStore.ShoppingCartAPI.Dtos
{
    public class ProductDto
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string PictureUrl { get; set; }
        public double Quantity { get; set; }
        public decimal CartQuantity { get; set; }

    }
}
