using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartStore.ProductsAPI.Data;
using SmartStore.ProductsAPI.Dtos;
using SmartStore.ProductsAPI.Entities;
using SmartStore.ProductsAPI.Helpers;
using SmartStore.UI.Dtos.Cart;

namespace SmartStore.ProductsAPI.Repository
{

    public class ProductRepository : IProductRepository
    {
        private readonly DbContextOptions<ProductsDbContext> _Context;


        public ProductRepository(DbContextOptions<ProductsDbContext> context)
        {
            _Context = context;
            //_mapper = new Mapper(new MappingProfile());
        }
        public async Task<IEnumerable<Product>> GetProducts()
        {
            using var _dbContext = new ProductsDbContext(_Context);
            var products = await _dbContext.Products.ToListAsync();
            return products;
        }
        public async Task<Product> GetProductById(int productId)
        {
            using var _dbContext = new ProductsDbContext(_Context);

            var product = await _dbContext.Products.FindAsync(productId);
            return product;

        }
        public async Task<Product> UpsertProduct(Product product)
        {
            using var _dbContext = new ProductsDbContext(_Context);


            if (product.Id > 0)
            {
                _dbContext.Products.Update(product);
            }
            else
            {
                await _dbContext.Products.AddAsync(product);
            }

            await _dbContext.SaveChangesAsync();

            return product;
        }

        public async Task<bool> DeleteProduct(int productId)
        {
            using var _dbContext = new ProductsDbContext(_Context);

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
            using var _dbContext = new ProductsDbContext(_Context);

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

        public async Task<bool> UpdateProductQuantity(ProductUpdateMessageDto productUpdateMessageDto)
        {
            using var _dbContext = new ProductsDbContext(_Context);

            try
            {

                foreach (var orderDetail in productUpdateMessageDto.OrderDetails)
                {
                    var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == orderDetail.ProductId);
                    if (product == null) return false;

                    product.Quantity -= orderDetail.Count;
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
