using SmartStore.CouponsAPI.Dtos;

namespace SmartStore.CouponsAPI.Repository
{
    public interface ICouponRepository
    {
        Task<IEnumerable<CouponDto>> GetAllCouponsAsync();
        Task<CouponDto> GetCouponByCode(string couponCode);
        Task<bool> CreateCouponAsync(CouponDto couponDto);
        Task<bool> UpdateCountAsync(CouponCountDetailsDto couponCountDetailsDto);
        Task<bool> DeleteCouponAsync(string couponCode);
    }
}
