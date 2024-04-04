using AutoMapper;
using GloboTicket.Grpc;
using GloboTicket.Services.ExternalPayment.Models;

namespace GloboTicket.Services.ExternalPayment.Profiles
{
    public class CheckoutProfile : Profile
    {
        public CheckoutProfile()
        {
            CreateMap<CheckoutOrderRequest, PaypalCheckoutRequest>().ReverseMap();
        }
        
    }
}
