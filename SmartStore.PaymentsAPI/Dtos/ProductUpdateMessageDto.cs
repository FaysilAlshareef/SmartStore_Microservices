using SmartStore.MessageBus;
using SmartStore.PaymentsAPI.Models;

namespace SmartStore.PaymentsAPI.Dtos
{
    public class ProductUpdateMessageDto:BaseMessage
    {
        public IEnumerable<OrderDetailsDto>? OrderDetails { get; set; }

    }
}
