using SmartStore.MessageBus;

namespace SmartStore.PaymentsAPI.Dtos
{
    public class PaymentUpdateMessageDto:BaseMessage
    {
        public int OrderId { get; set; }
        public string Email { get; set; }
        public bool Status { get; set; }
    }
}
