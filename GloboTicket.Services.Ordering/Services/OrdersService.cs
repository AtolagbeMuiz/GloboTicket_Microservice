using AutoMapper;
using GloboTicket.Grpc;
using GloboTicket.Services.Ordering.Entities;
using GloboTicket.Services.Ordering.Repositories;
using Grpc.Core;
using System;
using System.Threading.Tasks;

namespace GloboTicket.Services.Ordering.Services
{
    public class OrdersService : Orders.OrdersBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrdersService( IOrderRepository orderRepository, IMapper mapper)
        {
            this._orderRepository = orderRepository;
            _mapper = mapper;
        }

        public override async Task<CreateOrderResponse> CreateOrder(CreateOrderRequest createOrderRequest, ServerCallContext context)
        {
            var response = new CreateOrderResponse();

            var order = new Order
            {
                UserId = Guid.Parse(createOrderRequest.UserId),
                Id = createOrderRequest.Id,
                OrderTotal = createOrderRequest.OrderTotal,
                OrderPlaced = DateTime.Now,
                OrderPaid = false,
                BasketId = Guid.Parse(createOrderRequest.BasketId)
            };

           // var order = _mapper.Map<Order>(createOrderRequest);

            await _orderRepository.AddOrder(order);

            return response;
        }
    }
}
