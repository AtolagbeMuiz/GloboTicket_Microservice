using GloboTicket.Web.Models.Api;
using System.Threading.Tasks;
using System;
using GloboTicket.Web.Models;
using System.Net.Http;

namespace GloboTicket.Web.Services
{
    public class ExternalPaymentService : IExternalPaymentService
    {
        private readonly HttpClient _client;
       
        public ExternalPaymentService(HttpClient client)
        {
            this._client = client;
        }

        public async Task<string> CompleteOrder(string orderID)
        {
            var response = await _client.GetAsync($"/api/payment/{orderID}");
            if (response.IsSuccessStatusCode)
            {

                var orderResponse = response.Content.ReadAsStringAsync();
                var orderresponseResult = orderResponse.Result;

                return orderresponseResult;
                // return await response.ReadContentAs<PaymentCheckOutOrderResponse>();

            }
            else
            {
                throw new Exception("Something went wrong placing your order. Please try again.");
            }
        }
    }
}
