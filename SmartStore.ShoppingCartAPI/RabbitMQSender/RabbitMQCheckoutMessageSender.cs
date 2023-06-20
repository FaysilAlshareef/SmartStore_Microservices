using Newtonsoft.Json;
using RabbitMQ.Client;
using SmartStore.MessageBus;
using System.Text;

namespace SmartStore.ShoppingCartAPI.RabbitMQSender;

public class RabbitMQCheckoutMessageSender : IRabbitMQCheckoutMessageSender
{
    private readonly string _hostName;
    private readonly string _userName;
    private readonly string _password;
    private IConnection _connection;

    public RabbitMQCheckoutMessageSender()
    {
        _hostName = "Localhost";
        _userName = "guest";
        _password = "guest";
    }
    public void SendMessage(BaseMessage message, string queueName)
    {
        if (ConnectionExists())
        {
            using var channel = _connection.CreateModel();

            channel.QueueDeclare(queueName, false, false, false);
            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish("", queueName, null, body);

        }

    }
    private void CreateConnection()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password
            };
            _connection = factory.CreateConnection();
        }
        catch (Exception ex)
        {
            //log Exception
            throw;
        }
    }
    private bool ConnectionExists()
    {
        if (_connection != null)
        {
            return true;
        }
        CreateConnection();
        return _connection != null;
    }
}
