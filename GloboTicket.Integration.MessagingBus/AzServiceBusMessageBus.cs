using GloboTicket.Integration.Messages;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GloboTicket.Integration.MessagingBus
{
    public class AzServiceBusMessageBus : IMessageBus
    {
        //private string connectionString = "Endpoint=sb://globoticket-messgingbus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=NuJyuurV2/uyX3dyRMvSaITIB/YG4o+vl+ASbA9rags=";
        private string connectionString = "Endpoint=sb://globoticket-messgingbus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=NuJyuurV2/uyX3dyRMvSaITIB/YG4o+vl+ASbA9rags=;TransportType=AmqpWebSockets";

        public async Task PublishMessage(IntegrationBaseMessage message, string topicName)
        {
            try
            {
                ISenderClient topicClient = new TopicClient(connectionString, topicName);

                var jsonMessage = JsonConvert.SerializeObject(message);
                var serviceBusMessage = new Message(Encoding.UTF8.GetBytes(jsonMessage))
                {
                    CorrelationId = Guid.NewGuid().ToString()
                };

                await topicClient.SendAsync(serviceBusMessage);
                Console.WriteLine($"Sent message to {topicClient.Path}");
                await topicClient.CloseAsync();

            }
            catch (Exception ex)
            {

                throw;
            }
          
        }
    }
}
