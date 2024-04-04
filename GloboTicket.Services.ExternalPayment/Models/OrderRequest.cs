using System;

namespace GloboTicket.Services.ExternalPayment.Models
{
    public class OrderRequest
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string BasketId { get; set; }
        public double OrderTotal { get; set; }
    }
}
