using SmartStore.MessageBus;

namespace SmartStore.PaymentsAPI.RabbitMQSender;

public interface IRabbitMQPaymentResultMessageSender
{
    void SendMessage(BaseMessage message, string queueName = "");
}
