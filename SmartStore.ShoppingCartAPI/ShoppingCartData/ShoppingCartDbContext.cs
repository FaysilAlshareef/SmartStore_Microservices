using Microsoft.EntityFrameworkCore;
using SmartStore.ShoppingCartAPI.Models;

namespace SmartStore.ShoppingCartAPI.ShoppingCartData
{
    public class ShoppingCartDbContext:DbContext
    {
        public ShoppingCartDbContext(DbContextOptions<ShoppingCartDbContext> options):base(options) 
        {
            
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<CartHeader> CartHeaders { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }
        //public DbSet<Cart> Carts { get; set; }

    }
}
