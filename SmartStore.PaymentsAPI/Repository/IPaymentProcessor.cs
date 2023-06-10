using SmartStore.PaymentsAPI.Dtos;

namespace SmartStore.PaymentsAPI.Repository
{
    public interface IPaymentProcessor
    {
        Task<bool> ProcessPayment(PaymentRequestMessageDto paymentMessage);
    }
}
