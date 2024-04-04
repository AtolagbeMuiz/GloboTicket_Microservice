namespace GloboTicket.Services.ShoppingBasket.Models
{
    public class PaymentCheckOutOrderResponse
    {
        public string Id { get; set; }
        public string Status { get; set; }
    }

    public class PaymentCheckOutOrderRequest
    {
        public double BasketTotal { get; set; }
        public string BasketId { get; set; }
        public string UserId { get; set; }
    }
}
