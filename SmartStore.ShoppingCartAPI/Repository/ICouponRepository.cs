

using SmartStore.ShoppingCartAPI.Dtos;

namespace SmartStore.ShoppingCartAPI.Repository
{
    public interface ICouponRepository
    {    
        Task<CouponDto> GetCouponByCode(string couponCode);
      
    }
}
