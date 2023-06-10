using SmartStore.UI.Models;
using SmartStore.UI.Services.Interfaces;
using SmartStore.UI.Dtos.Cart;
using SmartStore.UI.Dtos.Coupon;

namespace SmartStore.UI.Services
{

    public class CartService : BaseService, ICartService
    {
        public CartService(IHttpClientFactory httpClient) : base(httpClient) { }

        public async Task<T> GetCartByUserIdAsync<T>(string userId, string token = null)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.GET,
                ApiUrl = SD.ShoppingCartApiUrl + $"/api/cart/GetCart/{userId}",
                AccessToken = token
            });
        }

        public async Task<T> AddCartAsync<T>(CartDto cartDto, string token = null)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                ApiUrl = SD.ShoppingCartApiUrl + "/api/cart/AddCart",
                AccessToken = token
            });
        }

        public async Task<T> UpdateCartAsync<T>(CartDto cartDto, string token = null)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                ApiUrl = SD.ShoppingCartApiUrl + "/api/cart/UpdateCart",
                AccessToken = token
            });
        }

        public async Task<T> RemoveFromCartAsync<T>(int cartDetailsId, string token = null)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDetailsId,
                ApiUrl = SD.ShoppingCartApiUrl + "/api/cart/RemoveFromCart",
                AccessToken = token
            });
        }
        public async Task<T> UpdateCountAsync<T>(CountDetailsDto countDetailsDto, string token = null)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = countDetailsDto,
                ApiUrl = SD.ShoppingCartApiUrl + "/api/cart/UpdateCount",
                AccessToken = token
            });
        }
        public async Task<T> ClearCartAsync<T>(string userId, string token = null)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = userId,
                ApiUrl = SD.ShoppingCartApiUrl + "/api/cart/ClearCart",
                AccessToken = token
            });
        }

        public async Task<T> ApplyAndRemoveCoupon<T>(CartDto cartDto,  string token = null)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                ApiUrl = SD.ShoppingCartApiUrl + "/api/cart/Coupon",
                AccessToken = token
            });
        }

        public async Task<T> CheckoutAsync<T>(CartHeaderDto cartHeader, string token = null)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = cartHeader,
                ApiUrl = SD.ShoppingCartApiUrl + "/api/cart/Checkout",
                AccessToken = token
            }); 
        }
    }
}

