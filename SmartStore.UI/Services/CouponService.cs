using SmartStore.UI.Dtos.Coupon;
using SmartStore.UI.Models;
using SmartStore.UI.Services.Interfaces;

namespace SmartStore.UI.Services
{
    public class CouponService :BaseService, ICouponService
    {
        private readonly IHttpClientFactory _httpClient;

        public CouponService(IHttpClientFactory httpClient) : base(httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T> CreateCouponAsync<T>(CouponDto couponDto, string token = null)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = couponDto,
                ApiUrl = $"{SD.CouponApiUrl}/api/coupon",
                AccessToken = token
            });
        }

        public async Task<T> DeleteCouponAsync<T>(string couponCode, string token = null)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.DELETE,                
                ApiUrl =  $"{SD.CouponApiUrl}/api/coupon/{couponCode}",
                AccessToken = token
            });
        }

        public async Task<T> GetAllCouponsAsync<T>(string token = null)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.GET,                
                ApiUrl = $"{SD.CouponApiUrl}/api/coupon",
                AccessToken = token
            });
        }

       

        public async Task<T> GetCouponByCode<T>(string couponCode, string token = null)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.GET,              
                ApiUrl = $"{SD.CouponApiUrl}/api/coupon/{couponCode}",
                AccessToken = token
            });
        }

        public async Task<T> UpdateCountAsync<T>(CouponCountDto couponCountDetailsDto, string token = null)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = couponCountDetailsDto,
                ApiUrl = $"{SD.CouponApiUrl}/api/coupon",
                AccessToken = token
            });
        }
    }
}
