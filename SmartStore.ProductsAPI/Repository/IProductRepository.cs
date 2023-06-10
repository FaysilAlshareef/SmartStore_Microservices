using SmartStore.ProductsAPI.Dtos;
using SmartStore.UI.Dtos.Cart;

namespace SmartStore.ProductsAPI.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductDto>> GetProducts();

        Task<ProductDto> GetProductById(int productId);

        Task<ProductDto> UpsertProduct(ProductDto productDto);

        Task<bool> UpdateQuantity(IEnumerable<CartDetailsDto> cartDetailsDtos);
        Task<bool> DeleteProduct(int productId);
    }
}
