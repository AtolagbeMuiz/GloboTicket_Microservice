using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using GloboTicket.Services.ExternalPayment.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Grpc.Net.Client;
using GloboTicket.Grpc;
using AutoMapper;

namespace GloboTicket.Services.ExternalPayment.Services
{
    public class PaypalService : IPaypalService
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        
        public PaypalService(IConfiguration config, IMapper mapper)
        {
            _config = config;
            _mapper = mapper;
       
        }

        public async Task<PaypalCheckoutResponse>  CreateOrder(PaypalCheckoutRequest paypalCheckoutRequest)
        {
            //Get Config values
            var baseUrl = _config.GetValue<string>("ApiRoute:PaypalBaseUrl");
            string PayPalClientId = _config.GetValue<string>("PayPalCredentials:SandboxClientId");
            string PayPalSecretKey = _config.GetValue<string>("PayPalCredentials:SandboxClientSecret");

            string AccessTokenEndpoint = _config.GetValue<string>("ApiRoute:GetAccessToken");
            var url = baseUrl + AccessTokenEndpoint;

            //TempData["baseurl"] = baseUrl;


            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(
                        System.Text.ASCIIEncoding.ASCII.GetBytes(
                           $"{PayPalClientId}:{PayPalSecretKey}")));

            var dict = new Dictionary<string, string>();
            dict.Add("Content-Type", "application/x-www-form-urlencoded");

            var req = new HttpRequestMessage(HttpMethod.Post, url + "?grant_type=client_credentials")
            {
                Content = new FormUrlEncodedContent(dict)
            };
            HttpResponseMessage resp = await client.SendAsync(req);

            client.Dispose();

            if (resp.StatusCode == System.Net.HttpStatusCode.OK && resp.IsSuccessStatusCode)
            {
                //////fetch cart items
                //var items = cart.GetCartItems();

                //var list = new List<Item>();
                //foreach (var it in items)
                //{
                //    list.Add(new Item
                //    {
                //        name = it.ProductName,
                //        description = it.ProductName,
                //        quantity = it.Quantity,
                //        unit_amount = new UnitAmount { currency_code = "GBP", value = it.Amount.ToString() }
                //    });
                //}

                var contentResponse = resp.Content.ReadAsStringAsync();
                var responseResult = contentResponse.Result;

                //serialize result into object model
                var authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseResult);


                PayPalOrder order = new PayPalOrder
                {
                    intent = "CAPTURE",
                    invoice_id = paypalCheckoutRequest.BasketId,
                    reference_id = paypalCheckoutRequest.BasketId,
                    purchase_units = new[] {
                        new PurchaseUnit
                        {
                            amount = new Amount
                            {
                                currency_code = "GBP",
                                value = paypalCheckoutRequest.BasketTotal.ToString(),

                               breakdown = new Breakdown()
                                {
                                    item_total = new ItemTotal()
                                    {
                                        currency_code = "GBP",
                                        value =  paypalCheckoutRequest.BasketTotal.ToString()
                                    }
                                }
                            },
                            items = new List<object>()

                        }
                    }
                };



                //get order checkout endpoint route from appsettings
                string checkoutOrderEndpoint = _config.GetValue<string>("ApiRoute:CheckoutOrder");


                using (var _client = new HttpClient())
                {
                    //TempData["access_token"] = authResponse.access_token;
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.access_token);
                    var requestHeader = new Dictionary<string, string>();
                    requestHeader.Add("Content-Type", "application/json");

                    StringContent content = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    //call checkout order endpoint
                    var oderEndpoint = baseUrl + checkoutOrderEndpoint;
                    HttpResponseMessage response = await _client.PostAsync(oderEndpoint, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var orderResponse = response.Content.ReadAsStringAsync();
                        var orderresponseResult = orderResponse.Result;

                        var res = JsonConvert.DeserializeObject<PaypalCheckoutResponse>(orderresponseResult);

                        //-----------------------------
                        var orderServiceChannel = GrpcChannel.ForAddress("https://localhost:5005");

                        OrderService orderService = new OrderService(new Orders.OrdersClient(orderServiceChannel));

                        var orderRequest = new OrderRequest
                        {
                            Id =  res.Id,
                            UserId = paypalCheckoutRequest.UserId.ToString(),
                            BasketId = paypalCheckoutRequest.BasketId.ToString(),
                            OrderTotal = paypalCheckoutRequest.BasketTotal
                        };

                        //var result = _mapper.Map<Basket>(orderRequest);

                        var checkOutOrderResponse = await orderService.CreateOrder(orderRequest);


                        return res;
                    }


                }

            }

            return null;
        }
    }
}
