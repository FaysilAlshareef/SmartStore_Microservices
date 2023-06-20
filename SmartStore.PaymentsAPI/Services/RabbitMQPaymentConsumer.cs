using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SmartStore.MessageBus;
using SmartStore.MessageBus.Interfaces;
using SmartStore.PaymentsAPI.Dtos;
using SmartStore.PaymentsAPI.RabbitMQSender;
using SmartStore.PaymentsAPI.Repository;
using System.Text;

namespace SmartStore.PaymentsAPI.Services;

public class RabbitMQPaymentConsumer : BackgroundService
{
    private readonly IPaymentProcessor _paymentProcessor;
    private readonly IConfiguration _configuration;
    private readonly IRabbitMQPaymentResultMessageSender _rabbitMQPaymentResultMessageSender;

    private readonly string _paymentRequestQueue;
    private readonly string _productUpdateQueue;
    private readonly string _hostName;
    private readonly string _userName;
    private readonly string _password;
    private IConnection _connection;
    private IModel _channel;

    public RabbitMQPaymentConsumer(
                    IPaymentProcessor paymentProcessor,
        IConfiguration configuration,
        IRabbitMQPaymentResultMessageSender rabbitMQPaymentResultMessageSender
        )
    {
        _paymentProcessor = paymentProcessor;
        _configuration = configuration;
        _rabbitMQPaymentResultMessageSender = rabbitMQPaymentResultMessageSender;
        _hostName = "Localhost";
        _userName = "guest";
        _password = "guest";
        _paymentRequestQueue = _configuration["PaymentRequestQueue"];
        _productUpdateQueue = _configuration["ProductUpdateQueue"];
        var factory = new ConnectionFactory
        {
            HostName = _hostName,
            UserName = _userName,
            Password = _password
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(_paymentRequestQueue, false, false, false);

    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (ch, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            var paymentRequestMessageDto = JsonConvert.DeserializeObject<PaymentRequestMessageDto>(content);
            HandleMessage(paymentRequestMessageDto).GetAwaiter().GetResult();
            _channel.BasicAck(ea.DeliveryTag, false);
        };
        _channel.BasicConsume(_paymentRequestQueue, false, consumer);
        return Task.CompletedTask;
    }

    private async Task HandleMessage(PaymentRequestMessageDto paymentRequestMessageDto)
    {
        var result = _paymentProcessor.ProcessPayment(paymentRequestMessageDto);


        var PaymentUpdateMessageDto = new PaymentUpdateMessageDto()
        {
            OrderId = paymentRequestMessageDto.OrderId,
            Status = true,
            Email = paymentRequestMessageDto.Email
        };
        if (result.Result == true)
        {
            var productUpdateMessageDto = new ProductUpdateMessageDto()
            {
                OrderDetails = paymentRequestMessageDto.OrderDetails,

            };
            try
            {
                _rabbitMQPaymentResultMessageSender.SendMessage(productUpdateMessageDto, _productUpdateQueue);
            }
            catch (Exception)
            {
                // log ex

            }
        }


        try
        {
            _rabbitMQPaymentResultMessageSender.SendMessage(PaymentUpdateMessageDto);
        }
        catch (Exception)
        {

            // log ex
        }
    }
}
