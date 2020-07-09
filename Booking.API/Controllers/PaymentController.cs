using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Stripe;
using Booking.Core.Services.Interfaces;
using Booking.Infrastructure.Payments.Stripe;
using Booking.Core.Domain.DTOs;
using Booking.Core.Domain.Entities;

namespace Booking.API.Controllers
{
    [Authorize]
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _configuration;

        public PaymentController(IPaymentService paymentService, IConfiguration configuration)
        {
            _paymentService = paymentService;
            _configuration = configuration;
        }

        [Authorize(Roles = Role.Customer)]
        [HttpPost("calculate-reservation-price")]
        public async Task<ActionResult<ShowPriceDto>> CalculateReservationPrice([FromForm]CalculatePriceDto calculatePriceDto, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _paymentService.CalculateReservationPriceAsync(calculatePriceDto, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.Customer)]
        [HttpPost("process-payment")]
        public async Task<IActionResult> ProcessPayment([FromForm]ProcessPaymentDto processPaymentDto, CancellationToken cancellationToken)
        {
            try
            {
                await _paymentService.ProcessPaymentAsync(processPaymentDto, cancellationToken);              
                return Ok(new { message = "Payment was successfully processed." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("webhook-check-credit-card-payment-status")]
        public async Task<IActionResult> CheckCreditCardPaymentStatus(CancellationToken cancellationToken)
        {
            try
            {
                var webHookSecret = _configuration[StripeConfig.STRIPE_WEEBHOOK_SECRET_KEY];

                var json = new StreamReader(HttpContext.Request.Body).ReadToEnd();
                var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], webHookSecret, throwOnApiVersionMismatch: true);

                await _paymentService.CheckPaymentStatusAsync(stripeEvent, cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
       
        [Authorize(Roles = Role.ServiceProvider)]
        [HttpGet("get-provider-transactions")]
        public async Task<ActionResult<IEnumerable<ProviderTransactionsDto>>> GetProviderTransactions(CancellationToken cancellationToken)
        {
            try
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var transactions = await _paymentService.GetTransactionsByProvider(userId, cancellationToken);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }        
    }
}
