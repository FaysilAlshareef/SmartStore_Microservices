using Microsoft.EntityFrameworkCore;
using SmartStore.OrdersAPI.Models;

namespace SmartStore.OrdersAPI.Data
{
    public class OrdersDbContext:DbContext
    {
        public OrdersDbContext(DbContextOptions<OrdersDbContext> options ):base(options)
        {
            
        }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
    }
}
