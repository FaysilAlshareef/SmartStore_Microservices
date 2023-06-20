using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SmartStore.MessageBus;
using SmartStore.MessageBus.Interfaces;
using SmartStore.OrdersAPI.Dtos;
using SmartStore.OrdersAPI.Repository;
using System.Text;

namespace SmartStore.OrdersAPI.Services;

public class RabbitMQPaymentConsumer : BackgroundService
{

    private readonly OrderRepository _orderRepository;
    private readonly IConfiguration _configuration;

    private string queueName = "";

    private readonly string _hostName;
    private readonly string _userName;
    private readonly string _password;
    private IConnection _connection;
    private IModel _channel;
    private const string _fanoutExchangeName = "Fanout_Payment_Update_Exchange";


    public RabbitMQPaymentConsumer(
                   OrderRepository orderRepository,
        IConfiguration configuration
        )
    {

        _orderRepository = orderRepository;
        _configuration = configuration;

        _hostName = "Localhost";
        _userName = "guest";
        _password = "guest";

        var factory = new ConnectionFactory
        {
            HostName = _hostName,
            UserName = _userName,
            Password = _password
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(_fanoutExchangeName, ExchangeType.Fanout, false);
        queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queueName, _fanoutExchangeName, "");
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (ch, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            var paymentUpdateMessageDto = JsonConvert.DeserializeObject<PaymentUpdateMessageDto>(content);
            HandleMessage(paymentUpdateMessageDto).GetAwaiter().GetResult();
            _channel.BasicAck(ea.DeliveryTag, false);
        };
        _channel.BasicConsume(queueName, false, consumer);
        return Task.CompletedTask;
    }

    private async Task HandleMessage(PaymentUpdateMessageDto paymentUpdateMessageDto)
    {


        try
        {
            await _orderRepository.UpdateOrderPaymentStatus(paymentUpdateMessageDto.OrderId, paymentUpdateMessageDto.Status);


        }
        catch (Exception)
        {

            // log ex 
        }
    }
}
