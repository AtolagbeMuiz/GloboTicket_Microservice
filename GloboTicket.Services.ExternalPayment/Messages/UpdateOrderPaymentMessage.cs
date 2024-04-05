using GloboTicket.Integration.Messages;

namespace GloboTicket.Services.ExternalPayment.Messages
{
    public class UpdateOrderPaymentMessage:IntegrationBaseMessage
    {
        public string OrderId { get; set; }
        public bool OrderPaid { get; set; }
    }
}
