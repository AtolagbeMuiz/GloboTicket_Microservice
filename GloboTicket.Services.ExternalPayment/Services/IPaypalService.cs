using GloboTicket.Services.ExternalPayment.Models;
using System.Threading.Tasks;

namespace GloboTicket.Services.ExternalPayment.Services
{
    public interface IPaypalService
    {
        Task<PaypalCheckoutResponse> CreateOrder(PaypalCheckoutRequest req);
    }
}
