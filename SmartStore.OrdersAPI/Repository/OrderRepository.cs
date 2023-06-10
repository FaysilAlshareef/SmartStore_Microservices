using Microsoft.EntityFrameworkCore;
using SmartStore.OrdersAPI.Data;
using SmartStore.OrdersAPI.Models;

namespace SmartStore.OrdersAPI.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbContextOptions<OrdersDbContext> _context;

        public OrderRepository(DbContextOptions<OrdersDbContext> context)
        {
            _context = context;
        }
        public async Task<IEnumerable<OrderHeader>> GetAllOrders()
        {
            await using var _dbContext = new OrdersDbContext(_context);
            return await _dbContext.OrderHeaders.ToListAsync();

        }



        public async Task<IEnumerable<OrderHeader>> GetUserOrders(string userId)
        {
            await using var _dbContext = new OrdersDbContext(_context);

            return await _dbContext.OrderHeaders
                    .Where(oh => oh.UserId == userId).ToListAsync();
        }

        public async Task<bool> AddOrder(OrderHeader orderHeader)
        {
            try
            {
                await using var _dbContext = new OrdersDbContext(_context);

                await _dbContext.OrderHeaders.AddAsync(orderHeader);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {

                return false;
            }

        }


        public async Task UpdateOrderPaymentStatus(int orderHeaderId, bool status)
        {
            await using var _dbContext = new OrdersDbContext(_context);

            var orderHeader = await _dbContext.OrderHeaders
                 .FirstOrDefaultAsync(oh => oh.OrderHeaderId == orderHeaderId);
            if (orderHeader != null)
            {
                try
                {
                    orderHeader.PaymentStatus = status;
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception)
                {

                    throw;
                }
               
            }

        }

        public async Task<OrderHeader> GetOrderById(int orderHeaderId)
        {
            await using var _dbContext = new OrdersDbContext(_context);

            return await _dbContext.OrderHeaders
                              .FirstOrDefaultAsync(oh => oh.OrderHeaderId == orderHeaderId);

        }
    }
}
