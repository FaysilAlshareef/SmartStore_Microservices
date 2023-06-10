using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartStore.MessageBus.Interfaces;
using SmartStore.ShoppingCartAPI.Dtos;
using SmartStore.ShoppingCartAPI.Messages;
using SmartStore.ShoppingCartAPI.Repository;

namespace SmartStore.ShoppingCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMessageBus _messageBus;
        private readonly ICouponRepository _couponRepository;
        private readonly ShoppingCartResponseDto _response;
        private readonly IConfiguration _configuration;

        public CartController(
            ICartRepository cartRepository,
            IMessageBus messageBus,
            ICouponRepository couponRepository,
            ShoppingCartResponseDto response,
            IConfiguration configuration
            )
        {
            _cartRepository = cartRepository;
            _messageBus = messageBus;
            _couponRepository = couponRepository;
            _response = response;
            _configuration = configuration;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<object> GetCart(string userId)
        {
            try
            {
                var cartDto = await _cartRepository.GetCartByUserIdAsync(userId);
                _response.Result = cartDto;
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>
                {
                    e.ToString()
                };
            }
            return _response;
        }

        [HttpPost("AddCart")]
        public async Task<object> AddCart(CartDto cartDto)
        {
            try
            {
                var result = await _cartRepository.UpsertCartAsync(cartDto);
                _response.Result = result;
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>
                {
                    e.ToString()
                };
            }
            return _response;
        }

        [HttpPost("UpdateCart")]
        public async Task<object> UpdateCart(CartDto cartDto)
        {
            try
            {
                var result = await _cartRepository.UpsertCartAsync(cartDto);
                _response.Result = result;

            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>
                {
                    e.ToString()
                };
            }
            return _response;
        }

        [HttpPost("RemoveFromCart")]
        public async Task<object> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                var isSuccess = await _cartRepository.RemoveFromCartAsync(cartDetailsId);
                _response.Result = isSuccess;

            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>
                {
                    e.ToString()
                };
            }
            return _response;
        }

        [HttpPost("UpdateCount")]
        public async Task<object> UpdateCountAsync(CountDetailsDto countDetailsDto)
        {
            try
            {
                var isSuccess = await _cartRepository.UpdateCountAsync(countDetailsDto);
                _response.Result = isSuccess;

            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>
                {
                    e.ToString()
                };
            }
            return _response;
        }

        [HttpPost("ClearCart")]
        public async Task<object> ClearCart([FromBody] string userId)
        {
            try
            {
                var isSuccess = await _cartRepository.ClearCart(userId);
                _response.Result = isSuccess;

            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>
                {
                    e.ToString()
                };
            }
            return _response;
        }
        [HttpPost("Coupon")]
        public async Task<object> ApplyOrRemoveCoupon(CartDto cartDto)
        {
            try
            {
                var isSuccess = await _cartRepository.ApplyAndRemoveCoupon(cartDto.CartHeader.UserId, cartDto.CartHeader.CouponCode);
                _response.Result = isSuccess;
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>
                {
                    e.ToString()
                };
            }
            return _response;
        }
        [HttpPost("Checkout")]
        public async Task<object> Checkout(CheckoutMessageDto checkoutMessageDto)
        {
            try
            {
                var cartDto = await _cartRepository.GetCartByUserIdAsync(
                    checkoutMessageDto.UserId);

                if (cartDto == null) return BadRequest();

                if (!string.IsNullOrEmpty(checkoutMessageDto.CouponCode))
                {
                    var coupon = await _couponRepository.GetCouponByCode(checkoutMessageDto.CouponCode);
                    double total = 0;
                    foreach (var detail in cartDto.CartDetails)
                    {
                        total += (detail.Product.Price * detail.Count);
                    }
                    if (checkoutMessageDto.DiscountTotal !=
                        (decimal)Math.Round(coupon.DiscountAmount * total, 2))
                    {
                        _response.IsSuccess = false;
                        _response.ErrorMessages.Add("Coupon discount amount has been changed, please confirm");
                        _response.DisplayMessage = "Coupon discount amount has been changed, please confirm";

                    }
                    if (coupon.NumberOfCoupon == coupon.NumberOfUsedCoupon)
                    {
                        _response.IsSuccess = false;
                        _response.ErrorMessages.Add("Coupon Has Expired, please confirm");
                        _response.DisplayMessage = "Coupon Has Expired, please confirm";

                    }

                    if (!_response.IsSuccess)
                        return _response;

                }
                checkoutMessageDto.CartDetails = cartDto.CartDetails;
                _response.Result = cartDto;
                // Logic Code to Add message to process order via azure service bus
                checkoutMessageDto.TopicName = _configuration["CheckoutMessageQueue"];
                var message = new List<MessageBus.BaseMessage>();
                message.Add(checkoutMessageDto);
                await _messageBus.PublishMessage(message);
                await _cartRepository.ClearCart(checkoutMessageDto.UserId);
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>
                {
                    e.ToString()
                };
            }
            return _response;
        }
    }

}
