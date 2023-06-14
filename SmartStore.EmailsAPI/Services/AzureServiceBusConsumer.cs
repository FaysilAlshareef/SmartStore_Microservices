using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using SmartStore.EmailsAPI.Dtos;
using SmartStore.EmailsAPI.Repository;
using SmartStore.MessageBus.Interfaces;


using System.Collections.Generic;
using System.Text;

namespace SmartStore.EmailsAPI.Services
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly EmailRepository _emailRepository;
        private readonly IConfiguration _configuration;
        private readonly string _serviceBusConnectionString;

        private readonly string _emailSubscription;
        private readonly string _paymentUpdateTopic;

        private readonly ServiceBusProcessor _emailProcessor;

        public AzureServiceBusConsumer(
            EmailRepository emailRepository,
            IConfiguration configuration)
        {
            _emailRepository = emailRepository;
            _configuration = configuration;
            _serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            _emailSubscription = _configuration.GetValue<string>("EmailSubscription");
            _paymentUpdateTopic = _configuration.GetValue<string>("PaymentUpdateTopic");

            var client = new ServiceBusClient(_serviceBusConnectionString);

            _emailProcessor = client.CreateProcessor(_paymentUpdateTopic, _emailSubscription);

        }
        public async Task Start()
        {


            _emailProcessor.ProcessMessageAsync += OnPaymentUpdateMessageReceived;
            _emailProcessor.ProcessErrorAsync += ErrorHandler;

            await _emailProcessor.StartProcessingAsync();
        }


        public async Task Stop()
        {

            await _emailProcessor.StopProcessingAsync();
            await _emailProcessor.DisposeAsync();
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
                await _emailRepository.SendAndLogEmail(paymentUpdateMessage);

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
