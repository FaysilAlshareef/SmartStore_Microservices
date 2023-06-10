using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using SmartStore.MessageBus.Interfaces;
using SmartStore.OrdersAPI.Data;
using SmartStore.OrdersAPI.Dtos;
using SmartStore.OrdersAPI.Models;
using SmartStore.OrdersAPI.Repository;
using System.Collections.Generic;
using System.Text;

namespace SmartStore.OrdersAPI.Services
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly OrderRepository _orderRepository;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        private readonly string _serviceBusConnectionString;
        private readonly string _checkoutMessageTopic;
        private readonly string _checkoutMessageQueue;
        private readonly string _checkoutMessageSubscription;
        private readonly string _paymentUpdateSubscription;
        private readonly string _paymentUpdateTopic;
        private readonly string _paymentRequestTopic;
        private readonly ServiceBusProcessor _checkoutProcessor;
        private readonly ServiceBusProcessor _paymentUpdateProcessor;

        public AzureServiceBusConsumer(
            OrderRepository orderRepository,
            IMessageBus messageBus,
            IConfiguration configuration)
        {
            _orderRepository = orderRepository;
            _messageBus = messageBus;
            _configuration = configuration;
            _serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            //_checkoutMessageTopic = _configuration.GetValue<string>("CheckoutMessageTopic");
            _checkoutMessageQueue = _configuration.GetValue<string>("CheckoutMessageQueue");
            _checkoutMessageSubscription = _configuration.GetValue<string>("CheckoutMessageSubscription");
            _paymentUpdateSubscription = _configuration.GetValue<string>("PaymentUpdateSubscription");
            _paymentUpdateTopic = _configuration.GetValue<string>("PaymentUpdateTopic");
            _paymentRequestTopic = _configuration.GetValue<string>("PaymentRequestTopic");
            var client = new ServiceBusClient(_serviceBusConnectionString);
            //_checkoutProcessor = client.CreateProcessor(_checkoutMessageTopic, _checkoutMessageSubscription);
            _checkoutProcessor = client.CreateProcessor(_checkoutMessageQueue);

            _paymentUpdateProcessor = client.CreateProcessor(_paymentUpdateTopic, _paymentUpdateSubscription);

        }
        public async Task Start()
        {
            _checkoutProcessor.ProcessMessageAsync += OnCheckoutMessageReceived;
            _checkoutProcessor.ProcessErrorAsync += ErrorHandler;
            await _checkoutProcessor.StartProcessingAsync();

            _paymentUpdateProcessor.ProcessMessageAsync += OnPaymentUpdateMessageReceived;
            _paymentUpdateProcessor.ProcessErrorAsync += ErrorHandler;

            await _paymentUpdateProcessor.StartProcessingAsync();
        }


        public async Task Stop()
        {
            await _checkoutProcessor.StopProcessingAsync();
            await _checkoutProcessor.DisposeAsync();

            await _paymentUpdateProcessor.StopProcessingAsync();
            await _paymentUpdateProcessor.DisposeAsync();
        }

        private Task ErrorHandler(ProcessErrorEventArgs arg)
        {
            Console.WriteLine(arg.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnPaymentUpdateMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            var paymentUpdateMessage = JsonConvert.DeserializeObject<PaymentUpdateMessageDto>(body);

            try
            {
                await _orderRepository.UpdateOrderPaymentStatus(paymentUpdateMessage.OrderId, paymentUpdateMessage.Status);

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception)
            {

                throw;
            }
        }
        private async Task OnCheckoutMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            if (message != null)
            {
                var body = Encoding.UTF8.GetString(message.Body);
                var checkoutMessageDto = JsonConvert
                                            .DeserializeObject<CheckoutMessageDto>(body);

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
                    var paymentMessage = new List<MessageBus.BaseMessage>();
                    paymentMessage.Add(paymentRequestMessageDto);
                    await _messageBus.PublishMessage(paymentMessage);
                    await args.CompleteMessageAsync(args.Message);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
