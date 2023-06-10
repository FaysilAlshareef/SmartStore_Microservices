using Microsoft.EntityFrameworkCore;
using SmartStore.ProductsAPI.Entities;
using System.Reflection;

namespace SmartStore.ProductsAPI.Data
{
    public class ProductsDbContext:DbContext
    {
        public ProductsDbContext(DbContextOptions<ProductsDbContext> options):base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().HasData(new Product
            {
                Id = 1,
                Name = "MSI GF63 Thin Gaming Laptop",
                Price = 4600,
                Description = "MSI GF63 Thin Gaming Laptop - Intel Core I5 - 8GB RAM - 256GB SSD - 15.6-inch FHD - 4GB GPU - Windows 10 - Black (English Keyboard).",
                PictureUrl = "/images/products/1.jpg",
                Quantity = 20,
                Category = "Laptops"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                Id = 2,
                Name = "DELL G3 15-3500 Gaming Laptop",
                Price = 3500,
                Description = "DELL G3 15-3500 Gaming Laptop - Intel Core I5-10300H - 8GB RAM - 256GB SSD + 1TB HDD - 15.6-inch FHD - 4GB GTX 1650 GPU - Ubuntu - Black.",
                PictureUrl = "/images/products/2.jpg",
                Quantity = 25,
                Category = "Laptops"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                Id = 3,
                Name = "Samsung UA43T5300",
                Price = 1600,
                Description = "Samsung UA43T5300 - 43-inch Full HD Smart TV With Built-In Receiver.",
                PictureUrl = "/images/products/3.jpg",
                Quantity = 50,
                Category = "Televisions"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                Id = 4,
                Name = "LG 43LM6370PVA",
                Price = 660,
                Description = "LG 43LM6370PVA - 43-inch Full HD Smart TV With Built-in Receiver.",
                PictureUrl = "/images/products/4.jpg",
                Quantity = 35,
                Category = "Televisions"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                Id = 5,
                Name = "Samsung Galaxy A12",
                Price = 950,
                Description = "Samsung Galaxy A12 - 6.4-inch 128GB/4GB Dual SIM Mobile Phone - White.",
                PictureUrl = "/images/products/5.jpg",
                Quantity = 70,
                Category = "Mobile Phones"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                Id = 6,
                Name = "Apple iPhone 12 Pro Max",
                Price = 4300,
                Description = "Apple iPhone 12 Pro Max Dual SIM with FaceTime - 256GB - Pacific Blue.",
                PictureUrl = "/images/products/6.jpg",
                Quantity = 65,
                Category = "Mobile Phones"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                Id = 7,
                Name = "OPPO Realme 8 Pro Case",
                Price = 1500,
                Description = "OPPO Realme 8 Pro Case, Dual Layer PC Back TPU Bumper Hybrid No-Slip Shockproof Cover For OPPO Realme 8 / Realme 8 Pro 4G.",
                PictureUrl = "/images/products/7.jpg",
                Quantity = 40,
                Category = "Mobile Accessories"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                Id = 8,
                Name = "Galaxy Z Flip3 5G Case",
                Price = 4000,
                Description = "Galaxy Z Flip3 5G Case, Slim Luxury Electroplate Frame Crystal Clear Back Protective Case Cover For Samsung Galaxy Z Flip 3 5G Purple.",
                PictureUrl = "/images/products/8.jpg",
                Quantity = 40,
                Category = "Mobile Accessories"
            });
        }

        public DbSet<Product> Products { get; set; }
       
    }
}
