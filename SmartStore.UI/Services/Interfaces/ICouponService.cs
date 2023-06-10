using SmartStore.UI.Dtos.Coupon;

namespace SmartStore.UI.Services.Interfaces
{
    public interface ICouponService
    {
        Task<T> GetAllCouponsAsync<T>( string token = null);
        Task<T> GetCouponByCode<T>(string couponCode, string token = null);
        Task<T> CreateCouponAsync<T>(CouponDto couponDto, string token = null);
        Task<T> UpdateCountAsync<T>(CouponCountDto couponCountDto, string token = null);
        Task<T> DeleteCouponAsync<T>(string couponCode, string token = null);
       
    }
}
