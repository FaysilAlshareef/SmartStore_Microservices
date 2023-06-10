using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using SmartStore.MessageBus.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartStore.MessageBus
{
    public class AzureServiceBus : IMessageBus
    {
        private string connectionString = "Endpoint=sb://storeservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=hXSY4DlcmPN4govrfk3yCBmA70t6BhkvW+ASbCGfwH0=";

        public async Task PublishMessage(IEnumerable<BaseMessage> messages)
        {

            await using var client = new ServiceBusClient(connectionString);


            foreach (var message in messages)
            {
                ServiceBusSender sender = client.CreateSender(message.TopicName);

                var jsonMessage = JsonConvert.SerializeObject(message, new JsonSerializerSettings()
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    Formatting = Formatting.Indented
                });
                ServiceBusMessage finalMessage = new(Encoding.UTF8.GetBytes(jsonMessage))
                {
                    CorrelationId = Guid.NewGuid().ToString()
                };
                await sender.SendMessageAsync(finalMessage);

            }
           
            await client.DisposeAsync();
        }
    }
}
