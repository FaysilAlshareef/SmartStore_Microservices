using SmartStore.OrdersAPI.Models;

namespace SmartStore.OrdersAPI.Repository
{
    public interface IOrderRepository
    {
        Task<bool> AddOrder(OrderHeader orderHeader);
        Task UpdateOrderPaymentStatus(int orderHeaderId,bool status);
        
        Task<IEnumerable<OrderHeader>> GetAllOrders();
        Task<IEnumerable<OrderHeader>> GetUserOrders(string userId);
        Task<OrderHeader> GetOrderById(int orderHeaderId);
    }
}
