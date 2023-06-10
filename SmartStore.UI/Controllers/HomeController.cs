using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartStore.UI.Dtos;
using SmartStore.UI.Dtos.Cart;
using SmartStore.UI.Dtos.Products;
using SmartStore.UI.Models;
using SmartStore.UI.Services.Interfaces;
using System.Diagnostics;
using System.Security.Claims;

namespace SmartStore.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(
            ILogger<HomeController> logger,
            IConfiguration configuration,
            IProductService productService,
            ICartService cartService,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _logger = logger;
            _configuration = configuration;
            _productService = productService;
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto> products = new();
            var response = await _productService.GetAllProductsAsync<ResponseDto>("");
            if (response != null && response.IsSuccess)
            {
                products = JsonConvert.DeserializeObject<List<ProductDto>>(
                    Convert.ToString(response.Result));

                if (User.Identity.IsAuthenticated)
                {
                    _httpContextAccessor.HttpContext.Session.SetInt32("count", GetCartCount().Result);
                }
            }
            return View(products);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id.Value == null)
                return BadRequest();

            ProductDto product = new();
            var response = await _productService.GetProductAsync<ResponseDto>(id.Value, "");
            if (response != null && response.IsSuccess)
            {
                product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            }
            return View(product);
        }
        [HttpPost]
        [ActionName("Details")]
        [Authorize]
        public async Task<IActionResult> DetailsPost(ProductDto productDto)
        {
            CartDto cartDto = new()
            {
                CartHeader = new CartHeaderDto()
                {
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                }
            };

            CartDetailsDto cartDetailsDto = new()
            {
                Count = (int)productDto.CartQuantity,
                ProductId = productDto.Id,
            };

            var response = await _productService
                .GetProductAsync<ResponseDto>(productDto.Id, "");
            if (response != null && response.IsSuccess)
            {
                cartDetailsDto.Product = JsonConvert
                    .DeserializeObject<ProductDto>(response.Result.ToString());
            }

            List<CartDetailsDto> cartDetailsDtos = new();
            cartDetailsDtos.Add(cartDetailsDto);

            cartDto.CartDetails = cartDetailsDtos;

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var addToCartResponse = await _cartService
                .AddCartAsync<ResponseDto>(cartDto, accessToken);

            if (addToCartResponse != null && addToCartResponse.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return View(productDto);
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


        [Authorize]
        public IActionResult Login()
        {
            //var accessToken = await HttpContext.GetTokenAsync("access_token");
            return RedirectToAction("Index");
        }
        [Authorize]
        public IActionResult Register()
        {
            var url = _configuration["ApiUrls:IdentityServer"];

            return Redirect($"{url}/Account/Register/Index");
        }
        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}