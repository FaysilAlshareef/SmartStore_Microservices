using Newtonsoft.Json;
using RabbitMQ.Client;
using SmartStore.MessageBus;
using SmartStore.PaymentsAPI.RabbitMQSender;
using System.Text;

namespace SmartStore.PaymentsAPI.RabbitMQSender;

public class RabbitMQPaymentResultMessageSender : IRabbitMQPaymentResultMessageSender
{
    private readonly string _hostName;
    private readonly string _userName;
    private readonly string _password;
    private IConnection _connection;
    private const string _fanoutExchangeName = "Fanout_Payment_Update_Exchange";


    public RabbitMQPaymentResultMessageSender()
    {
        _hostName = "Localhost";
        _userName = "guest";
        _password = "guest";
    }
    public void SendMessage(BaseMessage message, string queueName = "")
    {
        if (ConnectionExists())
        {
            using var channel = _connection.CreateModel();

            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            if (!string.IsNullOrEmpty(queueName))
            {
                channel.QueueDeclare(queueName, false, false, false);

                channel.BasicPublish("", queueName, null, body);

            }
            else
            {
                channel.ExchangeDeclare(_fanoutExchangeName, ExchangeType.Fanout, false);
                channel.BasicPublish(_fanoutExchangeName, "", null, body);

            }
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
