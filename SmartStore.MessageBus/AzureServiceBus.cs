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
        private string connectionString = "Endpoint=sb://smartstoreservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=mUUO+i/PGy1mBTIBW1FzWMHSQF+JntW98+ASbIbSEmM=";

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
