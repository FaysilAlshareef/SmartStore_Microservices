using AutoMapper;
using SmartStore.CouponsAPI.Dtos;
using SmartStore.CouponsAPI.Models;

namespace SmartStore.CouponsAPI.Helpers
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Coupon,CouponDto>().ReverseMap();
        }
    }
}
