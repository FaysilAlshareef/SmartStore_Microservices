using SmartStore.MessageBus;

namespace SmartStore.OrdersAPI.RabbitMQSender;

public interface IRabbitMQPaymentRequestMessageSender
{
    void SendMessage(BaseMessage message, string queueName);
}
