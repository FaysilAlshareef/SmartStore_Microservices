using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SmartStore.MessageBus.Interfaces;
using SmartStore.ProductsAPI.Dtos;
using SmartStore.ProductsAPI.Repository;
using System.Text;

namespace SmartStore.OrdersAPI.Services;

public class RabbitMQPaymentConsumer : BackgroundService
{
    private readonly ProductRepository _productRepository;
    private readonly IConfiguration _configuration;
    private readonly string _productUpdateTopic;

    private readonly string _hostName;
    private readonly string _userName;
    private readonly string _password;
    private IConnection _connection;
    private IModel _channel;

    public RabbitMQPaymentConsumer(
        ProductRepository productRepository,
        IConfiguration configuration
        )
    {

        _productRepository = productRepository;
        _configuration = configuration;
        _hostName = "Localhost";
        _userName = "guest";
        _password = "guest";
        _productUpdateTopic = _configuration.GetValue<string>("ProductUpdateQueue");

        var factory = new ConnectionFactory
        {
            HostName = _hostName,
            UserName = _userName,
            Password = _password
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(_productUpdateTopic, false, false, false);

    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (ch, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            var productUpdateMessageDto = JsonConvert.DeserializeObject<ProductUpdateMessageDto>(content);
            HandleMessage(productUpdateMessageDto).GetAwaiter().GetResult();
            _channel.BasicAck(ea.DeliveryTag, false);
        };
        _channel.BasicConsume(_productUpdateTopic, false, consumer);
        return Task.CompletedTask;
    }

    private async Task HandleMessage(ProductUpdateMessageDto productUpdateMessageDto)
    {

        try
        {
            _productRepository.UpdateProductQuantity(productUpdateMessageDto);
            //_rabbitMQPaymentRequestMessageSender.SendMessage(paymentRequestMessageDto, _configuration["PaymentRequestQueue"]);
        }
        catch (Exception)
        {

            throw;
        }
    }
}
