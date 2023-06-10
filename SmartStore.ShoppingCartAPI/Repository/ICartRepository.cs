using SmartStore.ShoppingCartAPI.Dtos;

namespace SmartStore.ShoppingCartAPI.Repository
{
    public interface ICartRepository
    {
        Task<CartDto> GetCartByUserIdAsync(string userId);
        Task<CartDto> UpsertCartAsync(CartDto cartDto);
        Task<bool> RemoveFromCartAsync(int cartDetailsId);
        Task<bool> UpdateCountAsync(CountDetailsDto countDetailsDto);
        Task<bool> ApplyAndRemoveCoupon(string userId, string couponCode = "");
        Task<bool> ClearCart(string userId);

    }
}
