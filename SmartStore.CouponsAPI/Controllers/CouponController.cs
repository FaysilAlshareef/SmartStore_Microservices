using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartStore.CouponsAPI.Dtos;
using SmartStore.CouponsAPI.Repository;

namespace SmartStore.CouponsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponRepository _couponRepository;
        private readonly CouponResponseDto _response;

        public CouponController(ICouponRepository couponRepository, CouponResponseDto response)
        {
            _couponRepository = couponRepository;
            _response = response;
        }
        [Authorize]

        [HttpGet]
        public async Task<ActionResult<CouponResponseDto>> GetAllCoupons()
        {
            try
            {
                var couponDtos = await _couponRepository.GetAllCouponsAsync();
                _response.Result = couponDtos;
            }
            catch (Exception e)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    e.ToString()
                };
            }
            return _response;
        }

        [HttpGet("{couponCode}")]
        public async Task<ActionResult<CouponResponseDto>> GetCoupon(string couponCode)
        {
            try
            {
                var couponDto = await _couponRepository.GetCouponByCode(couponCode);
                _response.Result = couponDto;
            }
            catch (Exception e)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    e.ToString()
                };
            }
            return _response;
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CouponResponseDto>> CreateCoupon(CouponDto couponDto)
        {
            try
            {
                var result = await _couponRepository.CreateCouponAsync(couponDto);
                if (result is true)
                {
                    _response.Result = result;
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Coupon Code is Exist");

                }
            }
            catch (Exception e)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    e.ToString()
                };
            }
            return _response;
        }
        [Authorize]

        [HttpPut]
        public async Task<ActionResult<CouponResponseDto>> UpdateNumberOfCoupon(CouponCountDetailsDto couponCountDetailsDto)
        {
            try
            {
                var result = await _couponRepository.UpdateCountAsync(couponCountDetailsDto);
                _response.Result = result;
            }
            catch (Exception e)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    e.ToString()
                };
            }
            return _response;
        }
        [Authorize]

        [HttpDelete("{couponCode}")]
        public async Task<ActionResult<CouponResponseDto>> DeleteCoupon(string couponCode)
        {
            try
            {
                var result = await _couponRepository.DeleteCouponAsync(couponCode);
                if (result is true)
                {
                    _response.Result = result;
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Coupon Code was using in orders");

                }
            }
            catch (Exception e)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    e.ToString()
                };
            }
            return _response;
        }
    }
}
