using System;

namespace GloboTicket.Services.Ordering.Entities
{
    public class Order
    {
        public string Id { get; set; }
        public Guid UserId { get; set; }
        public Guid BasketId { get; set; }
        public double OrderTotal { get; set; }
        public DateTime OrderPlaced { get; set; }
        public bool OrderPaid { get; set; }
    }
}
