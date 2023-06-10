using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartStore.UI.Dtos;
using SmartStore.UI.Dtos.Coupon;

using SmartStore.UI.Services.Interfaces;

namespace SmartStore.UI.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService) 
        {
            _couponService = couponService;
        }

        [Authorize]
        public async Task<IActionResult> CouponIndex()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _couponService.GetAllCouponsAsync<ResponseDto>(accessToken);

            var couponDto = new List<CouponDto>();
            if(response !=null && response.IsSuccess) 
            { 
                couponDto=JsonConvert
                    .DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result));
                foreach (var coupon in couponDto)
                {
                    coupon.DiscountAmount = coupon.DiscountAmount * 100;
                }
            }
            return View(couponDto);
        }

        public IActionResult CreateCoupon()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCoupon(CouponDto couponDto)
        {
            if (ModelState.IsValid)
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");


                couponDto.DiscountAmount = couponDto.DiscountAmount / 100;
                
                var response = await _couponService.CreateCouponAsync<ResponseDto>(couponDto, accessToken);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction("CouponIndex");
                }
            }
            return View(couponDto);
        }

        public async Task<IActionResult> DeleteCoupon(string? couponCode)
        {
            if (couponCode == null)
                return BadRequest();

            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var response = await _couponService.GetCouponByCode<ResponseDto>(couponCode, accessToken);

            if (response != null && response.IsSuccess)
            {
                var coupon = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));
                return View(coupon);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(CouponDto couponDto)
        {


            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var response = await _couponService.DeleteCouponAsync<ResponseDto>(couponDto.CouponCode, accessToken);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction("CouponIndex");
            }

            return View(couponDto);
        }
        public async Task<IActionResult> Decrease(int CouponId)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            CouponCountDto couponCountDto = new()
            {
                CouponId = CouponId,
                Action = "decrement",
                Amount = 1
            };
            var response = await _couponService
                                .UpdateCountAsync<ResponseDto>(couponCountDto, accessToken);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction("CouponIndex");
            }

            return RedirectToAction("CouponIndex"); ;
        }
        
        public async Task<IActionResult> Increase(int CouponId)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            CouponCountDto couponCountDto = new()
            {
                CouponId = CouponId,
                Action = "increment",
                Amount = 1
            };
            var response = await _couponService
                                .UpdateCountAsync<ResponseDto>(couponCountDto, accessToken);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction("CouponIndex");
            }

            return RedirectToAction("CouponIndex"); ;
        }
    }
}
