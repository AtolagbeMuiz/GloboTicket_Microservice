using Grpc.Core;
using GloboTicket.Grpc;
using System.Threading.Tasks;
using AutoMapper;
using GloboTicket.Services.ExternalPayment.Models;

namespace GloboTicket.Services.ExternalPayment.Services
{
    public class PaymentsService : Payments.PaymentsBase
    {
        private readonly IPaypalService _paypalService;
        private readonly IMapper _mapper;

        public PaymentsService(IPaypalService paypalService, IMapper mapper)
        {
            _paypalService = paypalService;
            _mapper = mapper;
           
        }

        public override async Task<CheckoutOrderResponse> CheckoutOrder(CheckoutOrderRequest request, ServerCallContext context)
        {
            var response = new CheckoutOrderResponse();

            var paypalCheckoutRequest = _mapper.Map<PaypalCheckoutRequest>(request);

            var order = await _paypalService.CreateOrder(paypalCheckoutRequest);

            if(order != null)
            {
                response.Response = new Response
                {
                    Id = order.Id,
                    Status = order.Status
                };
                return response;

            }
            return response;

        }
    }
}
