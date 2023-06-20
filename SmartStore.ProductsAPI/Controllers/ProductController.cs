using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartStore.ProductsAPI.Dtos;
using SmartStore.ProductsAPI.Entities;
using SmartStore.ProductsAPI.Repository;
using SmartStore.UI.Dtos.Cart;

namespace SmartStore.ProductsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductRepository _productRepository;
        private readonly IMapper _mapper;
        private ProductResponseDto _responseDto;

        public ProductsController(ProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _responseDto = new ProductResponseDto();
        }
        //Get All Products /api/products
        [HttpGet]
        public async Task<ProductResponseDto> GetAllProducts()
        {
            try
            {
                IEnumerable<Product> product =
                    await _productRepository.GetProducts();

                _responseDto.Result = _mapper.Map<IEnumerable<ProductDto>>(product);
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
                Product product = await _productRepository.GetProductById(id);

                _responseDto.Result = _mapper.Map<ProductDto>(product);
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
        [Authorize(Roles = "Admin")]
        public async Task<object> CreateProduct(ProductDto productDto)
        {
            try
            {
                if (productDto.Id == 0)
                {
                    Product model = await _productRepository.UpsertProduct(_mapper.Map<Product>(productDto));

                    _responseDto.Result = _mapper.Map<ProductDto>(model);
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
                    Product model = await _productRepository.UpsertProduct(_mapper.Map<Product>(productDto));

                    _responseDto.Result = _mapper.Map<ProductDto>(model);
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
                var result = _productRepository.UpdateQuantity(cartDetailsDtos);
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
