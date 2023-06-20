using SmartStore.MessageBus;

namespace SmartStore.ShoppingCartAPI.RabbitMQSender;

public interface IRabbitMQCheckoutMessageSender
{
    void SendMessage(BaseMessage message, string queueName);
}
