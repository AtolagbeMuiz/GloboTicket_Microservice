namespace GloboTicket.Services.Ordering.Messages
{
    public class UpdateOrderPaymentMessage
    {
        public string OrderId { get; set; }
        public bool OrderPaid { get; set; }
    }
}
