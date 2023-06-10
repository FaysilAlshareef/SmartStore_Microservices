using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartStore.UI.Dtos;
using SmartStore.UI.Dtos.Cart;
using SmartStore.UI.Dtos.Coupon;
using SmartStore.UI.Services;
using SmartStore.UI.Services.Interfaces;
using System.Security.Claims;

namespace SmartStore.UI.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly ICouponService _couponService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartController(IProductService productService,
            ICartService cartService,
            ICouponService couponService,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _productService = productService;
            _cartService = cartService;
            _couponService = couponService;
            _httpContextAccessor = httpContextAccessor;
        }
        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            if (User.Identity.IsAuthenticated)
            {
                _httpContextAccessor.HttpContext.Session.SetInt32("count", GetCartCount().Result);
            }
            return View(await LoadCartOfLoggedInUser());
        }

        public async Task<IActionResult> RemoveItem(int cartDetailId)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var resonse = await _cartService.RemoveFromCartAsync<ResponseDto>(cartDetailId, accessToken);
            if (resonse != null & resonse.IsSuccess)
            {
                TempData["SuccessMessage"] = "The Product Has Deleted";
                return RedirectToAction("CartIndex");
            }

            return View();
        }

        public async Task<IActionResult> Decrease(int DetailId)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            CountDetailsDto countDetailsDto = new()
            {
                CartDetailsId = DetailId,
                Action = "decrement",
                Amount = 1
            };
            var response = await _cartService
                                .UpdateCountAsync<ResponseDto>(countDetailsDto, accessToken);
            if (response != null && response.IsSuccess)           
                return RedirectToAction("CartIndex");
            else
                TempData["Message"] = "You Can't decrement Cart Quantity more";


            return RedirectToAction("CartIndex"); ;
        }

        public async Task<IActionResult> Increase(int DetailId)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            CountDetailsDto countDetailsDto = new()
            {
                CartDetailsId = DetailId,
                Action = "increment",
                Amount = 1
            };
            var response = await _cartService
                                .UpdateCountAsync<ResponseDto>(countDetailsDto, accessToken);
            if (response != null && response.IsSuccess)
                return RedirectToAction("CartIndex");
           

            return RedirectToAction("CartIndex"); ;
        }

        public async Task<IActionResult> ClearCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService
                                .ClearCartAsync<ResponseDto>(userId, accessToken);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction("CartIndex");
            }

            return RedirectToAction("CartIndex"); ;
        }

        public async Task<IActionResult> ApplyOrDeleteCoupon(CartDto cartDto)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            ResponseDto response = new();
            if (cartDto.CartHeader.CouponCode != "")
            {
                ResponseDto res = await _couponService.GetCouponByCode<ResponseDto>(
                        cartDto.CartHeader.CouponCode, accessToken);
                if (res.Result != null && res.IsSuccess)
                {
                    var coupon = JsonConvert.DeserializeObject<CouponDto>(
                        Convert.ToString(res.Result));

                    if (coupon is not null && coupon.NumberOfCoupon > coupon.NumberOfUsedCoupon)
                    {
                        //ApplyCoupon
                        response = await _cartService
                                 .ApplyAndRemoveCoupon<ResponseDto>(cartDto, accessToken);
                        TempData["SuccessMessage"] = "The Coupon Has Applied";
                    }
                    else
                        TempData["Message"] = "The Coupon Has expired";

                }
            }
            else
            {
                //RemoveCoupon
                response = await _cartService
                                    .ApplyAndRemoveCoupon<ResponseDto>(cartDto, accessToken);
            }
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction("CartIndex");
            }



            return RedirectToAction("CartIndex");
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartOfLoggedInUser());
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var response = await _cartService
                    .CheckoutAsync<ResponseDto>(cartDto.CartHeader, accessToken);
                
                if (response == null | !response.IsSuccess)
                {
                    TempData["Message"]=response.DisplayMessage; 
                    return RedirectToAction("Checkout");
                }
                _httpContextAccessor.HttpContext.Session.SetInt32("count", 0);
                //var prodcuts = cartDto.CartDetails.Select(c => c.Product);
                //await _productService.UpdateAsync()
                return RedirectToAction("Confirmation");
            }
            catch (Exception)
            {

                return View(cartDto);
            }
        }

        [Authorize]
        public IActionResult Confirmation()
        {
            return View();
        }
        public async Task<int> GetCartCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService
                .GetCartByUserIdAsync<ResponseDto>(userId, accessToken);

            CartDto cart = new();

            if (response != null && response.IsSuccess)
            {
                cart = JsonConvert
                    .DeserializeObject<CartDto>(Convert.ToString(response.Result));
            }
            int count = 0;
            if (cart.CartHeader != null)
            {
                count = cart.CartDetails.Select(cd => cd.Count).Sum();
            }
            return count;
        }
  

        private async Task<CartDto> LoadCartOfLoggedInUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService
                .GetCartByUserIdAsync<ResponseDto>(userId, accessToken);

            CartDto cart = new();

            if (response != null && response.IsSuccess)
            {
                cart = JsonConvert
                    .DeserializeObject<CartDto>(Convert.ToString(response.Result));
            }

            if (cart.CartHeader != null)
            {
                foreach (var detail in cart.CartDetails)
                {
                    cart.CartHeader.OrderTotal += (detail.Product.Price * detail.Count);
                }

                if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    ResponseDto res = await _couponService.GetCouponByCode<ResponseDto>(
                        cart.CartHeader.CouponCode, accessToken);
                    if (res != null && res.IsSuccess)
                    {
                        var coupon = JsonConvert.DeserializeObject<CouponDto>(
                            Convert.ToString(res.Result));

                        if (coupon!=null && coupon.NumberOfCoupon > coupon.NumberOfUsedCoupon)
                        {
                            cart.CartHeader.DiscountTotal
                            = (decimal)Math.Round(coupon.DiscountAmount * (float)cart.CartHeader.OrderTotal, 2);
                            cart.CartHeader.OrderTotal -= cart.CartHeader.DiscountTotal;
                            cart.CartHeader.GrandTotal = cart.CartHeader.OrderTotal + cart.CartHeader.DiscountTotal;
                            return cart;
                        }
                       

                    }

                }

                cart.CartHeader.GrandTotal = cart.CartHeader.OrderTotal;

            }

            return cart;
        }

    }
}
