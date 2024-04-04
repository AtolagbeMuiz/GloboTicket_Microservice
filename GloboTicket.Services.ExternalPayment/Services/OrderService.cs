using GloboTicket.Grpc;
using GloboTicket.Services.ExternalPayment.Models;
using Grpc.Core;
using System;
using System.Threading.Tasks;

namespace GloboTicket.Services.ExternalPayment.Services
{
    public class OrderService
    {
        private readonly Orders.OrdersClient _ordersService;

        public OrderService(Orders.OrdersClient ordersService)
        {
            this._ordersService = ordersService;
        }

        public async Task<Task> CreateOrder(OrderRequest orderRequest)
        {
            try
            {

                CreateOrderRequest createOrderRequest = new CreateOrderRequest
                {
                    Id = orderRequest.Id,
                    OrderTotal = orderRequest.OrderTotal,
                    UserId = orderRequest.UserId,
                    BasketId = orderRequest.BasketId
                };

                CreateOrderResponse createOrderResponse = await _ordersService.CreateOrderAsync(createOrderRequest);

                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

    }
}
