using SmartStore.ProductsAPI.Dtos;
using SmartStore.ProductsAPI.Entities;
using SmartStore.UI.Dtos.Cart;

namespace SmartStore.ProductsAPI.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProducts();

        Task<Product> GetProductById(int productId);

        Task<Product> UpsertProduct(Product productDto);

        Task<bool> UpdateQuantity(IEnumerable<CartDetailsDto> cartDetailsDtos);
        Task<bool> DeleteProduct(int productId);
        Task<bool> UpdateProductQuantity(ProductUpdateMessageDto productUpdateMessageDto);
    }
}
