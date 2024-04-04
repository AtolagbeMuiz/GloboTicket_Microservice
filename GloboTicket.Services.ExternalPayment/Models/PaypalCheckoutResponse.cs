using System.Collections.Generic;

namespace GloboTicket.Services.ExternalPayment.Models
{
    public class PaypalCheckoutRequest
    {
        public double BasketTotal { get; set; }
        public string BasketId { get; set; }
        public object UserId { get; set; }
    }

    public class PaypalCheckoutResponse
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public object Links { get; set; }
    }

    public class AuthResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string app_id { get; set; }
        public double expires_in { get; set; }
        public string nonce { get; set; }
    }

    public class PayPalOrder
    {
        public string intent { get; set; }
        public Payer payer { get; set; }
        public PurchaseUnit[] purchase_units { get; set; }

        public string invoice_id { get; set; }

        public string reference_id { get; set; }
    }
    public class Payer
    {
        public string name { get; set; }
        public string email_address { get; set; }
    }

    public class PurchaseUnit
    {
        public Amount amount { get; set; }
        //public Item[] item { get; set; }
        public List<object> items { get; set; }

    }

    public class Amount
    {
        public string currency_code { get; set; }
        public string value { get; set; }

        public Breakdown breakdown { get; set; }
    }

    public class Breakdown
    {
        public ItemTotal item_total { get; set; }
    }

    public class ItemTotal
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }
}
