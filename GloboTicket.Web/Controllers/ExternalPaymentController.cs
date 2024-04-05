using GloboTicket.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace GloboTicket.Web.Controllers
{
    public class ExternalPaymentController : Controller
    {
        private readonly IExternalPaymentService _externalPayment;

        public ExternalPaymentController(IExternalPaymentService externalPayment)
        {
            this._externalPayment = externalPayment;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> CompleteOrder(string orderID)
        {
            if(orderID != null)
            {
                var response = await _externalPayment.CompleteOrder(orderID);

                if(response != null) 
                {
                    return Json(response);
                    // return null;
                }
               
            }
            return null;
        }


        [HttpGet]
        public IActionResult PaymentStatus(string status, string transactionId, string orderId, string InvoiceId, string paymentType)
        {
            if (!string.IsNullOrEmpty(status) && !string.IsNullOrEmpty(InvoiceId))
            {
                var paymentStatus = bool.Parse(status);
                if (paymentStatus == true)
                {
                    if (paymentType == "Paypal")
                    {
                        ViewBag.PaymentType = "Paypal";

                        ViewBag.transactionId = transactionId;
                        ViewBag.orderId = orderId;
                        ViewBag.InvoiceId = InvoiceId;
                        ViewBag.paymentStatus = true;
                    }

                }
                else
                {
                    ViewBag.paymentStatus = false;

                }

            }
            else
            {
                ViewBag.paymentStatus = false;

            }

            return View();
        }

    }
}
