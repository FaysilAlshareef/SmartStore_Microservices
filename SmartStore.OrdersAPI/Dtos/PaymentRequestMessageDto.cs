using SmartStore.MessageBus;
using SmartStore.OrdersAPI.Models;

namespace SmartStore.OrdersAPI.Dtos
{
    public class PaymentRequestMessageDto:BaseMessage
    {
        public int OrderId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public string ExpiryMonthYear { get; set; }
        public decimal OrderTotal { get; set; }
        public IEnumerable<OrderDetails>? OrderDetails { get; set; }
    }
}
