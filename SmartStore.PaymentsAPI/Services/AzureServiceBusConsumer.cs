using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using SmartStore.MessageBus;
using SmartStore.MessageBus.Interfaces;
using SmartStore.PaymentsAPI.Dtos;
using SmartStore.PaymentsAPI.Repository;
using SmartStore.PaymentsAPI.Services;
using System.Text;

namespace SmartStore.PaymentsAPI.Services
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly IPaymentProcessor _paymentProcessor;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        private readonly string _serviceBusConnectionString;
        private readonly string _paymentRequestTopic;
        private readonly string _paymentUpdateTopic;
        private readonly string _productUpdateTopic;
        private readonly string _storePaymentsSubscription;
        private ServiceBusProcessor _paymentRequestProcessor;

        public AzureServiceBusConsumer(
            IPaymentProcessor paymentProcessor,
            IMessageBus messageBus,
            IConfiguration configuration)
        {
            _paymentProcessor = paymentProcessor;
            _messageBus = messageBus;
            _configuration = configuration;
            _serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            _paymentRequestTopic = _configuration.GetValue<string>("PaymentRequestTopic");
            _paymentUpdateTopic = _configuration.GetValue<string>("PaymentUpdateTopic");
            _storePaymentsSubscription = _configuration.GetValue<string>("StorePaymentsSubscription");
            _productUpdateTopic = _configuration.GetValue<string>("ProductUpdateTopic");
           
            var client = new ServiceBusClient(_serviceBusConnectionString);
            _paymentRequestProcessor = client.CreateProcessor(_paymentRequestTopic, _storePaymentsSubscription);
        }
        public async Task Start()
        {
            _paymentRequestProcessor.ProcessMessageAsync += OnPaymentRequestMessageReceived;
            _paymentRequestProcessor.ProcessErrorAsync += ErrorHandler;
            
            await _paymentRequestProcessor.StartProcessingAsync();
        }


        public async Task Stop()
        {
            await _paymentRequestProcessor.StopProcessingAsync();
            await _paymentRequestProcessor.DisposeAsync();
        }

        private Task ErrorHandler(ProcessErrorEventArgs arg)
        {
            Console.WriteLine(arg.Exception.ToString());
            return Task.CompletedTask;
        }


        private async Task OnPaymentRequestMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            if (message != null)
            {
                var body = Encoding.UTF8.GetString(message.Body);
                var paymentRequestMessageDto = JsonConvert
                                            .DeserializeObject<PaymentRequestMessageDto>(body);

                if (paymentRequestMessageDto != null)
                {
                    var result = _paymentProcessor.ProcessPayment(paymentRequestMessageDto);
                    var paymentBaseMessages = new List<BaseMessage>();
                    paymentBaseMessages.Add(new PaymentUpdateMessageDto()
                    {
                        OrderId = paymentRequestMessageDto.OrderId,
                        Status = result.Result,
                        Email = paymentRequestMessageDto.Email,
                        TopicName= _paymentUpdateTopic
                    });
                    if (result.Result == true)
                    {
                        paymentBaseMessages.Add(new ProductUpdateMessageDto()
                        {
                            OrderDetails = paymentRequestMessageDto.OrderDetails,
                            TopicName= _productUpdateTopic
                            
                        });
                    }
                    try
                    {
                        await _messageBus.PublishMessage(paymentBaseMessages);
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
}
