using GloboTicket.Services.ShoppingBasket.Extensions;
using GloboTicket.Services.ShoppingBasket.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using GloboTicket.Grpc;
using GloboTicket.Services.ShoppingBasket.Migrations;
using PaymentCheckOutOrderResponse = GloboTicket.Services.ShoppingBasket.Models.PaymentCheckOutOrderResponse;

namespace GloboTicket.Services.ShoppingBasket.Services
{
    public class PaymentService
    {
        private readonly Payments.PaymentsClient paymentsService;
        public PaymentService(Payments.PaymentsClient paymentsService)
        {
            this.paymentsService = paymentsService;
        }

        public async Task<PaymentCheckOutOrderResponse> CheckoutOrder(PaymentCheckOutOrderRequest request)
        {
            try
            {
                CheckoutOrderRequest checkoutOrderRequest = new CheckoutOrderRequest 
                {
                    BasketTotal = request.BasketTotal,
                    BasketId = request.BasketId,
                    UserId = request.UserId
                   
                };

                CheckoutOrderResponse checkoutOrderResponse = await paymentsService.CheckoutOrderAsync(checkoutOrderRequest);

                var paymentCheckOutOrderResponse = new PaymentCheckOutOrderResponse
                {
                   Id = checkoutOrderResponse.Response.Id,
                   Status = checkoutOrderResponse.Response.Status
                };

                return paymentCheckOutOrderResponse;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}
