using SmartStore.MessageBus;
using SmartStore.PaymentsAPI.Models;

namespace SmartStore.PaymentsAPI.Dtos
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

        public IEnumerable<OrderDetailsDto>? OrderDetails { get; set; }

    }
}
