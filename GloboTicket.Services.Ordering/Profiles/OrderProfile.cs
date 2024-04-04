using AutoMapper;
using GloboTicket.Grpc;
using GloboTicket.Services.Ordering.Entities;

namespace GloboTicket.Services.Ordering.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, CreateOrderRequest>().ReverseMap();
        }
    }
}
