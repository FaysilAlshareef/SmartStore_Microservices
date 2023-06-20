using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SmartStore.MessageBus.Interfaces;
using SmartStore.OrdersAPI.Dtos;
using SmartStore.OrdersAPI.Models;
using SmartStore.OrdersAPI.RabbitMQSender;
using SmartStore.OrdersAPI.Repository;
using System.Text;

namespace SmartStore.OrdersAPI.Services;

public class RabbitMQCheckoutConsumer : BackgroundService
{
    private readonly OrderRepository _orderRepository;
    private readonly IConfiguration _configuration;
    private readonly IRabbitMQPaymentRequestMessageSender _rabbitMQPaymentRequestMessageSender;
    private readonly string _paymentRequestTopic;

    private readonly string _hostName;
    private readonly string _userName;
    private readonly string _password;
    private IConnection _connection;
    private IModel _channel;

    public RabbitMQCheckoutConsumer(
        OrderRepository orderRepository,
        IConfiguration configuration,
        IRabbitMQPaymentRequestMessageSender rabbitMQPaymentRequestMessageSender
        )
    {
        _orderRepository = orderRepository;
        _configuration = configuration;
        _rabbitMQPaymentRequestMessageSender = rabbitMQPaymentRequestMessageSender;
        _hostName = "Localhost";
        _userName = "guest";
        _password = "guest";
        _paymentRequestTopic = _configuration.GetValue<string>("PaymentRequestTopic");

        var factory = new ConnectionFactory
        {
            HostName = _hostName,
            UserName = _userName,
            Password = _password
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare("CheckoutMessageQueue", false, false, false);

    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (ch, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            var checkoutMessageDto = JsonConvert.DeserializeObject<CheckoutMessageDto>(content);
            HandleMessage(checkoutMessageDto).GetAwaiter().GetResult();
            _channel.BasicAck(ea.DeliveryTag, false);
        };
        _channel.BasicConsume("CheckoutMessageQueue", false, consumer);
        return Task.CompletedTask;
    }

    private async Task HandleMessage(CheckoutMessageDto checkoutMessageDto)
    {
        OrderHeader orderHeader = new()
        {
            UserId = checkoutMessageDto.UserId,
            FirstName = checkoutMessageDto.FirstName,
            LastName = checkoutMessageDto.LastName,
            OrderDetails = new List<OrderDetails>(),
            CardNumber = checkoutMessageDto.CardNumber,
            CouponCode = checkoutMessageDto.CouponCode,
            CVV = checkoutMessageDto.CVV,
            DiscountTotal = checkoutMessageDto.DiscountTotal,
            Email = checkoutMessageDto.Email,
            ExpiryMonthYear = checkoutMessageDto.ExpiryMonthYear,
            OrderDate = DateTime.Now,
            OrderTotal = checkoutMessageDto.OrderTotal,
            PaymentStatus = false,
            Phone = checkoutMessageDto.Phone,
            OrderDeliveryDate = checkoutMessageDto.PaymentDate
        };

        foreach (var item in checkoutMessageDto.CartDetails)
        {
            OrderDetails orderDetails = new()
            {
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                ProductPrice = (decimal)item.Product.Price,
                Count = item.Count
            };

            orderHeader.OrderDetails.Add(orderDetails);
        };

        await _orderRepository.AddOrder(orderHeader);
        var paymentRequestMessageDto = new PaymentRequestMessageDto()
        {
            Name = $"{orderHeader.FirstName} {orderHeader.LastName}",
            Email = orderHeader.Email,
            CardNumber = orderHeader.CardNumber,
            CVV = orderHeader.CVV,
            ExpiryMonthYear = orderHeader.ExpiryMonthYear,
            OrderId = orderHeader.OrderHeaderId,
            OrderTotal = orderHeader.OrderTotal,
            OrderDetails = orderHeader.OrderDetails,
            TopicName = _paymentRequestTopic,
        };
        try
        {
            _rabbitMQPaymentRequestMessageSender.SendMessage(paymentRequestMessageDto, _configuration["PaymentRequestQueue"]);
        }
        catch (Exception)
        {

            throw;
        }
    }
}
