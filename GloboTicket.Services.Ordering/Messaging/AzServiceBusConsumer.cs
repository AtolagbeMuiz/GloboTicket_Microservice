using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Configuration;
using GloboTicket.Services.Ordering.Messages;
using GloboTicket.Services.Ordering.Repositories;
using System;
using Microsoft.Azure.ServiceBus.Core;

namespace GloboTicket.Services.Ordering.Messaging
{
    public class AzServiceBusConsumer : IAzServiceBusConsumer
    {
        private readonly string subscriptionName = "globoticketorder";

        private readonly IReceiverClient _updateOrderMessageReceiverClient;


        private readonly string updateOrderMessageTopic;

        private readonly IConfiguration _configuration;
        private readonly OrderRepository _orderRepository;

        public AzServiceBusConsumer(IConfiguration configuration, OrderRepository orderRepository )
        {
            this._orderRepository = orderRepository;
            this._configuration = configuration;

            var serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            updateOrderMessageTopic = _configuration.GetValue<string>("UpdateOrderMessageTopic");

            _updateOrderMessageReceiverClient = new SubscriptionClient(serviceBusConnectionString, updateOrderMessageTopic, subscriptionName);

        }


        public void Start()
        {
            var messageHandlerOptions = new MessageHandlerOptions(OnServiceBusException) { MaxConcurrentCalls = 4 };


            _updateOrderMessageReceiverClient.RegisterMessageHandler(OnOrderPaymentUpdateReceived, messageHandlerOptions);
        }

        private async Task OnOrderPaymentUpdateReceived(Message message, CancellationToken arg2)
        {
            var body = Encoding.UTF8.GetString(message.Body);//json from service bus
            UpdateOrderPaymentMessage orderPaymentUpdateMessage =
                JsonConvert.DeserializeObject<UpdateOrderPaymentMessage>(body);

            await _orderRepository.UpdateOrderPaymentStatus(orderPaymentUpdateMessage.OrderId, orderPaymentUpdateMessage.OrderPaid);
        }

        private Task OnServiceBusException(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine(exceptionReceivedEventArgs);

            return Task.CompletedTask;
        }

        public void Stop()
        {
        }

    }



}
