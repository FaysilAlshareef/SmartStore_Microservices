using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartStore.UI.Dtos;
using SmartStore.UI.Dtos.Products;

using SmartStore.UI.Services.Interfaces;

namespace SmartStore.UI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IBlobService _blobService;

        public ProductController(IProductService productService, IBlobService blobService)
        {
            _productService = productService;
            _blobService = blobService;
        }
        public async Task<IActionResult> Index()
        {
            var products = new List<ProductDto>();
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.GetAllProductsAsync<ResponseDto>(accessToken);

            if (response != null && response.IsSuccess)
            {
                products = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            return View(products);
        }

        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var imageUrl = await _blobService
                                    .UploadBlob(productDto.Image.FileName, productDto.Image, "images");
                productDto.PictureUrl = imageUrl;
                var response = await _productService.CreateAsync<ResponseDto>(productDto, accessToken);
                if (response != null && response.IsSuccess)
                {

                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        public async Task<IActionResult> UpdateProduct(int? id, string view = "UpdateProduct")
        {
            if (id.Value == null)
                return BadRequest();
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var response = await _productService.GetProductAsync<ResponseDto>(id.Value, accessToken);

            if (response != null && response.IsSuccess)
            {
                var product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(view, product);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(ProductDto productDto, [FromRoute] int? id)
        {

            if (ModelState.IsValid)
            {
                if (id.Value != productDto.Id)
                    return BadRequest();

                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var imageUrl = await _blobService
                        .UploadBlob(productDto.Image.FileName, productDto.Image, "images");
                productDto.PictureUrl = imageUrl;
                var response = await _productService.UpdateAsync<ResponseDto>(productDto, accessToken);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(productDto);
        }


        public async Task<IActionResult> DeleteProduct(int? id)
        {

            return await UpdateProduct(id, "DeleteProduct");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(ProductDto productDto)
        {


            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var response = await _productService.DeleteAsync<ResponseDto>(productDto.Id, accessToken);
            if (response != null && response.IsSuccess)
            {
                // Delete blob
                if (productDto.PictureUrl.Contains("/"))
                {
                    var pos = productDto.PictureUrl.LastIndexOf("/") + 1;
                    var blobName = productDto.PictureUrl[pos..];
                    await _blobService.DeleteBlob(blobName, "images");
                }
                return RedirectToAction("Index");
            }

            return View(productDto);
        }
    }
}
