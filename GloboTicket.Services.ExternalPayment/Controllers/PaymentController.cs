using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;

namespace GloboTicket.Services.ExternalPayment.Controllers
{
    [Route("api/payment")]
    [ApiController]

    public class PaymentController : ControllerBase
    {

        private readonly IConfiguration _config;

        public PaymentController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("{orderID}", Name = "CompleteOrder")]
        public async Task<IActionResult> CompleteOrder(string orderID)
        {
            //get needed data
            
            var baseUrl = _config.GetValue<string>("ApiRoute:PaypalBaseUrl");
            string PayPalClientId = _config.GetValue<string>("PayPalCredentials:SandboxClientId");
            string PayPalSecretKey = _config.GetValue<string>("PayPalCredentials:SandboxClientSecret");

            using (var _client = new HttpClient())
            {
                string captureOrder = _config.GetValue<string>("ApiRoute:CaptureOrder");

                //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                //_client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token}");
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(
                        System.Text.ASCIIEncoding.ASCII.GetBytes(
                           $"{PayPalClientId}:{PayPalSecretKey}")));

                StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync($"{baseUrl}{captureOrder}{orderID}/capture", content);
                if (response.IsSuccessStatusCode)
                {
                    var captureResponse = response.Content.ReadAsStringAsync();
                    var captureResponseResult = captureResponse.Result;

                    return Ok(captureResponseResult);
                }
                else
                {
                    var orderResponse = response.Content.ReadAsStringAsync();
                    var orderresponseResult = orderResponse.Result;

                    return Ok(orderresponseResult);
                }


            }


            //return View();
        }
    }
}
