using SmartStore.UI.Dtos.Cart;
using SmartStore.UI.Dtos.Products;

namespace SmartStore.UI.Services.Interfaces
{
    public interface IProductService
    {
        Task<T> GetAllProductsAsync<T>(string token);
        Task<T> GetProductAsync<T>(int id, string token);

        Task<T> CreateAsync<T>(ProductDto productDto, string token);
        Task<T> UpdateAsync<T>(ProductDto productDto, string token);
        Task<T> UpdateQuantityAsync<T>(IEnumerable<CartDetailsDto> cartDetailsDtos, string token);
        Task<T> DeleteAsync<T>(int id, string token);
    }
}
