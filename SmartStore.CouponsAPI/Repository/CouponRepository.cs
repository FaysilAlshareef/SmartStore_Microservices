using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartStore.CouponsAPI.Data;
using SmartStore.CouponsAPI.Dtos;
using SmartStore.CouponsAPI.Models;

namespace SmartStore.CouponsAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly CouponDbContext _dbContext;
        private readonly IMapper _mapper;

        public CouponRepository(CouponDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CouponDto>> GetAllCouponsAsync()
        {
            var coupons = await _dbContext.Coupons.ToListAsync();

            return _mapper.Map<IEnumerable<CouponDto>>(coupons);
        }

        public async Task<CouponDto> GetCouponByCode(string couponCode)
        {
            var coupon = await _dbContext.Coupons
                .FirstOrDefaultAsync(C => C.CouponCode == couponCode);
            return _mapper.Map<CouponDto>(coupon);
        }

        public async Task<bool> CreateCouponAsync(CouponDto couponDto)
        {
            var coupon = await _dbContext.Coupons
                .FirstOrDefaultAsync(C => C.CouponCode == couponDto.CouponCode);
            //check if couponCode is exist
            if (coupon != null) return false;

            coupon = _mapper.Map<Coupon>(couponDto);

            await _dbContext.Coupons.AddAsync(coupon);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;

        }

        public async Task<bool> DeleteCouponAsync(string couponCode)
        {
            var coupon = await _dbContext.Coupons
                .FirstOrDefaultAsync(C => C.CouponCode == couponCode);

            if (coupon != null && coupon.NumberOfUsedCoupon == 0)
            {
                try
                {
                    _dbContext.Coupons.Remove(coupon);
                    var result = await _dbContext.SaveChangesAsync();
                    return result > 0;
                }
                catch (Exception )
                {

                    return false;
                }
            }
            return false;
        }

        public async Task<bool> UpdateCountAsync(CouponCountDetailsDto couponCountDetailsDto)
        {
            var coupon = await _dbContext.Coupons
                   .FirstOrDefaultAsync(C => C.CouponId == couponCountDetailsDto.CouponId);
            if (coupon != null)
            {
                if (couponCountDetailsDto.Action == "decrement")
                {
                    if (coupon.NumberOfCoupon- couponCountDetailsDto.Amount >= coupon.NumberOfUsedCoupon)
                    {
                        coupon.NumberOfCoupon -= couponCountDetailsDto.Amount;
                        await _dbContext.SaveChangesAsync();
                    }
                   
                }
                if (couponCountDetailsDto.Action == "increment")
                {
                    coupon.NumberOfCoupon += couponCountDetailsDto.Amount;
                    await _dbContext.SaveChangesAsync();
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
