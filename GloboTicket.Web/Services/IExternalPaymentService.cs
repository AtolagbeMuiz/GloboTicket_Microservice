using System.Threading.Tasks;

namespace GloboTicket.Web.Services
{
    public interface IExternalPaymentService
    {
        Task<string> CompleteOrder(string orderId);
    }
}
