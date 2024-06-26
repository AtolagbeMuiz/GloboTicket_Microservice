﻿using AutoMapper;
//using GloboTicket.Integration.MessagingBus;
using GloboTicket.Services.ShoppingBasket.Messages;
using GloboTicket.Services.ShoppingBasket.Models;
using GloboTicket.Services.ShoppingBasket.Repositories;
using GloboTicket.Services.ShoppingBasket.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Coupon = GloboTicket.Services.ShoppingBasket.Models.Coupon;
using Grpc.Net.Client;
using GloboTicket.Grpc;
using Microsoft.Extensions.Configuration;

namespace GloboTicket.Services.ShoppingBasket.Controllers
{
    [Route("api/baskets")]
    [ApiController]
    public class BasketsController : ControllerBase
    {
        private readonly IBasketRepository basketRepository;
        private readonly IMapper mapper;
        private readonly IConfiguration _config;
        // private readonly IMessageBus messageBus;
        //private readonly DiscountService discountService;

        public BasketsController(IBasketRepository basketRepository, IMapper mapper, IConfiguration config) // IMessageBus messageBus //, DiscountService discountService)
        {
            this.basketRepository = basketRepository;
            this.mapper = mapper;
            _config = config;
            //this.messageBus = messageBus;
            // this.discountService = discountService;
        }

        [HttpGet("{basketId}", Name = "GetBasket")]
        public async Task<ActionResult<Basket>> Get(Guid basketId)
        {
            var basket = await basketRepository.GetBasketById(basketId);
            if (basket == null)
            {
                return NotFound();
            }

            var result = mapper.Map<Basket>(basket);
            result.NumberOfItems = basket.BasketLines.Sum(bl => bl.TicketAmount);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Basket>> Post(BasketForCreation basketForCreation)
        {
            var basketEntity = mapper.Map<Entities.Basket>(basketForCreation);

            basketRepository.AddBasket(basketEntity);
            await basketRepository.SaveChanges();

            var basketToReturn = mapper.Map<Basket>(basketEntity);

            return CreatedAtRoute(
                "GetBasket",
                new { basketId = basketEntity.BasketId },
                basketToReturn);
        }

        [HttpPut("{basketId}/coupon")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ApplyCouponToBasket(Guid basketId, Coupon coupon)
        {
            var basket = await basketRepository.GetBasketById(basketId);

            if (basket == null)
            {
                return BadRequest();
            }

            basket.CouponId = coupon.CouponId;
            await basketRepository.SaveChanges();

            return Accepted();
        }





        [HttpPost("checkout")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CheckoutBasketAsync([FromBody] BasketCheckout basketCheckout)
        {
            try
            {


                //based on basket checkout, fetch the basket lines from repo
                var basket = await basketRepository.GetBasketById(basketCheckout.BasketId);

                if (basket == null)
                {
                    return BadRequest();
                }

                BasketCheckoutMessage basketCheckoutMessage = mapper.Map<BasketCheckoutMessage>(basketCheckout);
                basketCheckoutMessage.BasketLines = new List<BasketLineMessage>();
                int total = 0;

                foreach (var b in basket.BasketLines)
                {
                    var basketLineMessage = new BasketLineMessage
                    {
                        BasketLineId = b.BasketLineId,
                        Price = b.Price,
                        TicketAmount = b.TicketAmount
                    };

                    total += b.Price * b.TicketAmount;

                    basketCheckoutMessage.BasketLines.Add(basketLineMessage);
                }

                //apply discountt by talking to the discount service
                Coupon coupon = null;

                var discountBaseUrl = _config.GetValue<string>("ApiConfigs:Discount:Uri");

                //var channel = GrpcChannel.ForAddress("https://localhost:5003");
                var channel = GrpcChannel.ForAddress(discountBaseUrl);

                DiscountService discountService = new DiscountService(new Discounts.DiscountsClient(channel));
                if (basket.CouponId.HasValue)
                    coupon = await discountService.GetCoupon(basket.CouponId.Value);

                if (coupon != null)
                {
                    basketCheckoutMessage.BasketTotal = total - coupon.Amount;
                }
                else
                {
                    basketCheckoutMessage.BasketTotal = total;
                }

                try
                {
                    //await messageBus.PublishMessage(basketCheckoutMessage, "checkoutmessage");

                    //about to call the payment service to checkout order
                    var externalPaymentBaseUrl = _config.GetValue<string>("ApiConfigs:ExternalPayment:Uri");

                    //var paymentServiceChannel = GrpcChannel.ForAddress("https://localhost:5004");
                    var paymentServiceChannel = GrpcChannel.ForAddress(externalPaymentBaseUrl);

                    PaymentService paymentService = new PaymentService(new Payments.PaymentsClient(paymentServiceChannel));

                    var paymentCheckOutOrderRequest = new PaymentCheckOutOrderRequest
                    {
                        UserId = basketCheckoutMessage.UserId.ToString(),
                        BasketId = basketCheckoutMessage.BasketId.ToString(),
                        BasketTotal = basketCheckoutMessage.BasketTotal
                    };

                    var checkOutOrderResponse = await paymentService.CheckoutOrder(paymentCheckOutOrderRequest);

                    //return checkOutOrderResponse;
                    await basketRepository.ClearBasket(basketCheckout.BasketId);
                    //return Accepted(basketCheckoutMessage);
                    return Accepted(checkOutOrderResponse);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.StackTrace);
            }
        }
    }
}
