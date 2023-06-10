using SmartStore.UI.Dtos.Cart;
using SmartStore.UI.Dtos.Products;
using SmartStore.UI.Models;
using SmartStore.UI.Services.Interfaces;

namespace SmartStore.UI.Services
{
    public class ProductService : BaseService, IProductService
    {
        public ProductService(IHttpClientFactory httpClient) : base(httpClient)
        {
        }

        public async Task<T> CreateAsync<T>(ProductDto productDto, string token)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType=SD.ApiType.POST,
                Data = productDto,
                ApiUrl= $"{SD.ProductsApiUrl}/api/product/",
                AccessToken=token                
            });
        }

        public async Task<T> DeleteAsync<T>(int id, string token)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.DELETE,               
                ApiUrl =   $"{SD.ProductsApiUrl}/api/product/{id}",
                AccessToken = token

            });
        }

        public async Task<T> GetAllProductsAsync<T>(string token)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.GET,
                ApiUrl = $"{SD.ProductsApiUrl}/api/product/",
                AccessToken = token

            });
        }

        public async Task<T> GetProductAsync<T>(int id, string token)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.GET,
                ApiUrl = $"{SD.ProductsApiUrl}/api/product/{id}",
                AccessToken = token

            });
        }

        public async Task<T> UpdateAsync<T>(ProductDto productDto, string token)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = productDto,
                ApiUrl = $"{SD.ProductsApiUrl}/api/product/",
                AccessToken = token
            });
        }

        public async Task<T> UpdateQuantityAsync<T>(IEnumerable<CartDetailsDto> cartDetailsDtos, string token)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = cartDetailsDtos,
                ApiUrl = $"{SD.ProductsApiUrl}/api/product/UpdateQuantity",
                AccessToken = token
            });
        }
    }
}
