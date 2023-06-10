using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartStore.ProductsAPI.Data;
using SmartStore.ProductsAPI.Dtos;
using SmartStore.ProductsAPI.Entities;
using SmartStore.UI.Dtos.Cart;

namespace SmartStore.ProductsAPI.Repository
{

    public class ProductRepository : IProductRepository
    {
        private readonly ProductsDbContext _dbContext;
        private readonly IMapper _mapper;

        public ProductRepository(ProductsDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            var products = await _dbContext.Products.ToListAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }
        public async Task<ProductDto> GetProductById(int productId)
        {
            var product = await _dbContext.Products.FindAsync(productId);
            return _mapper.Map<ProductDto>(product);

        }
        public async Task<ProductDto> UpsertProduct(ProductDto productDto)
        {
            var product=_mapper.Map<ProductDto,Product>(productDto);
            if(product.Id>0)
            {
                _dbContext.Products.Update(product);
            }
            else
            {
                await _dbContext.Products.AddAsync(product);
            }

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<Product,ProductDto>(product);    
        }

        public async Task<bool> DeleteProduct(int productId)
        {
            try
            {
                var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
                if (product == null) return false;

                _dbContext.Products.Remove(product);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
                
            }
        }

        public async Task<bool> UpdateQuantity(IEnumerable<CartDetailsDto> cartDetailsDtos)
        {
            
            try
            {
                foreach (var cartDetails in cartDetailsDtos)
                {
                    var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == cartDetails.ProductId);
                    if (product == null) return false;

                    product.Quantity -= cartDetails.Count;
                }
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {

                return false;
            }

        }
    }




}
