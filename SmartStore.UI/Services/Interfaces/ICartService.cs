using SmartStore.UI.Dtos.Cart;

namespace SmartStore.UI.Services.Interfaces
{
    public interface ICartService
    {
        Task<T> GetCartByUserIdAsync<T>(string userId, string token = null);
        Task<T> AddCartAsync<T>(CartDto cartDto, string token = null);
        Task<T> UpdateCartAsync<T>(CartDto cartDto, string token = null);
        Task<T> RemoveFromCartAsync<T>(int cartDetailsId, string token = null);
        Task<T> UpdateCountAsync<T>(CountDetailsDto countDetailsDto, string token = null);
        Task<T> ApplyAndRemoveCoupon<T>(CartDto cartDto, string token = null);
        Task<T> CheckoutAsync<T>(CartHeaderDto cartHeader, string token = null);
        Task<T> ClearCartAsync<T>(string userId, string token = null);
    }
}

