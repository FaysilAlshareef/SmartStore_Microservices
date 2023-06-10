using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartStore.ProductsAPI.Dtos;
using SmartStore.ProductsAPI.Repository;
using SmartStore.UI.Dtos.Cart;

namespace SmartStore.ProductsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private ProductResponseDto _responseDto;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _responseDto = new ProductResponseDto();
        }
        //Get All Products /api/products
        [HttpGet]
        public async Task<ProductResponseDto> GetAllProducts()
        {
            try
            {
                IEnumerable<ProductDto> productDtos =
                    await _productRepository.GetProducts();

                _responseDto.Result = productDtos;
            }
            catch (Exception ex)
            {

                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages
                    = new List<string> { ex.ToString() };
            }

            return _responseDto;
        }

        //Get Single Product /api/products/1
        [HttpGet]
        [Route("{id}")]        
        public async Task<object> GetProduct(int id)
        {
            try
            {
                ProductDto productDto = await _productRepository.GetProductById(id);

                _responseDto.Result = productDto;
            }
            catch (Exception ex)
            {

                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages
                    = new List<string> { ex.ToString() };
            }

            return _responseDto;
        }

        //Insert New Product /api/products
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<object> CreateProduct(ProductDto productDto)
        {
            try
            {
                if (productDto.Id == 0)
                {
                    ProductDto model = await _productRepository.UpsertProduct(productDto);

                    _responseDto.Result = model;
                    _responseDto.DisplayMessage = "Product has been created";
                }
                else
                {
                    _responseDto.IsSuccess = false;
                    _responseDto.DisplayMessage = "Error occurs while creating product!";
                    _responseDto.ErrorMessages = new() { "Error occurs while creating product!" };
                    return _responseDto;
                }

            }
            catch (Exception ex)
            {

                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages
                    = new List<string> { ex.ToString() };
            }

            return _responseDto;
        }

        //Update Product /api/products
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<object> UpdateProduct(ProductDto productDto)
        {
            try
            {
                if (productDto.Id > 0)
                {
                    ProductDto model = await _productRepository.UpsertProduct(productDto);

                    _responseDto.Result = model;
                    _responseDto.DisplayMessage = "Product has been updated";
                }
                else
                {
                    _responseDto.IsSuccess = false;
                    _responseDto.DisplayMessage = "Product id is not correct!";
                    _responseDto.ErrorMessages = new() { "Product id is not correct!" };
                    return _responseDto;
                }

            }
            catch (Exception ex)
            {

                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages
                    = new List<string> { ex.ToString() };
            }

            return _responseDto;
        }

        [HttpPut("UpdateQuantity")]
        public async Task<object> UpdateProductsQuantity(IEnumerable<CartDetailsDto> cartDetailsDtos)
        {
            try
            {
              var result=  _productRepository.UpdateQuantity(cartDetailsDtos);
                return true;
            }
            catch (Exception ex)
            {

                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages
                    = new List<string> { ex.ToString() };
            }
            return _responseDto;
        }

        //Delete Product /api/products/1
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<object> DeleteProduct(int id)
        {
            try
            {
                bool isSuccess = await _productRepository.DeleteProduct(id);
                if (!isSuccess)
                {
                    _responseDto.IsSuccess = false;
                    _responseDto.Result = isSuccess;
                    _responseDto.DisplayMessage = "Product is not found!";
                    _responseDto.ErrorMessages = new() { "Product is not found!" };
                    return _responseDto;
                }
                else
                {
                    _responseDto.Result = isSuccess;
                    _responseDto.DisplayMessage = "Product has been deleted";
                }

            }
            catch (Exception ex)
            {

                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages
                    = new List<string> { ex.ToString() };
            }

            return _responseDto;
        }
    }
}
