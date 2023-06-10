using SmartStore.EmailsAPI.Dtos;

namespace SmartStore.EmailsAPI.Repository;

public interface IEmailRepository
{
    Task SendAndLogEmail(PaymentUpdateMessageDto messageDto);
}
