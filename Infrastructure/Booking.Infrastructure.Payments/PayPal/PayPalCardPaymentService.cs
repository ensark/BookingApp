using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PayPal.Core;
using PayPal.v1.Payments;
using Booking.Core.Domain.DTOs;
using Booking.Infrastructure.Common.Interfaces;

namespace Booking.Infrastructure.Payments.PayPal
{
    public class PayPalCardPaymentService : IPayPalCardPaymentService
    {
        private const string CURRRENCY_GBP = "GBP";
        private const string PAYMENT_METHOD = "paypal";
        private const string QUANTITY = "1";
        private const string INTENT = "sale";

        private readonly ILogger<PayPalCardPaymentService> _logger;
        private readonly string _clientId;
        private readonly string _secretKey;

        private readonly PayPalHttpClient _payPalHttpClient;

        public PayPalCardPaymentService(ILogger<PayPalCardPaymentService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _clientId = configuration[PayPalConfig.PAYPAL_CLIENT_ID];
            _secretKey = configuration[PayPalConfig.PAYPAL_SECRET_KEY];

            var environment = new SandboxEnvironment(_clientId, _secretKey);

            _payPalHttpClient = new PayPalHttpClient(environment);
        }

        public async Task<PaymentResponseDto> CreateChargeAsync(ProcessPaymentDto proccessCardPaymentDto, CancellationToken cancellationToken)
        {
            try
            {
                string[] getCustomerData;
                string customerFirstName = "";
                string customerLastName = "";

                if (!string.IsNullOrEmpty(proccessCardPaymentDto.CardHolder))
                {
                    getCustomerData = proccessCardPaymentDto.CardHolder.Split(' ');
                    customerFirstName = getCustomerData[0];
                    customerLastName = getCustomerData[1];
                }

                var appointmentList = new List<Item>();
                var payPalApprovalLink = "";

                foreach (var appointment in proccessCardPaymentDto.Metadata)
                {
                    var item = new Item()
                    {
                        Name = appointment.Value,
                        Currency = CURRRENCY_GBP,
                        Price = proccessCardPaymentDto.AmountPerSession.ToString().Replace(',','.'),
                        Quantity = QUANTITY,
                        Description = proccessCardPaymentDto.Description,
                    };

                    appointmentList.Add(item);
                };

                Payment payment = new Payment()
                {
                    Intent = INTENT,
                    Payer = new Payer()
                    {
                        PaymentMethod = PAYMENT_METHOD,
                        PayerInfo = new PayerInformation
                        {
                            Email = proccessCardPaymentDto.CustomerEmail,
                            FirstName = customerFirstName,
                            LastName = customerLastName
                        },
                    },
                    Transactions = new List<Transaction>()
                    {
                        new Transaction()
                        {
                            Amount = new Amount()
                            {
                               Currency = CURRRENCY_GBP,
                               Total = proccessCardPaymentDto.TotalAmount.ToString().Replace(',','.'),
                            },
                            Description = proccessCardPaymentDto.Description,
                            ItemList = new ItemList()
                            {
                                Items = appointmentList
                            }
                        }
                    },
                    RedirectUrls = new RedirectUrls()
                    {                       
                        ReturnUrl = "http://localhost:5000/",
                        CancelUrl = "http://localhost:5000/"
                    },
                };
                
                var paymentRequest = new PaymentCreateRequest();
                paymentRequest.RequestBody(payment);

                var response = await _payPalHttpClient.Execute(paymentRequest);
                var result = response.Result<Payment>();
                
                foreach (var link in result.Links)
                {
                    if (link.Rel.Equals("approval_url"))
                        payPalApprovalLink = link.Href;                   
                }

                return new PaymentResponseDto
                {
                    Status = response.StatusCode,
                    PayPalApprovalLink = payPalApprovalLink,
                    Created = Convert.ToDateTime(result.CreateTime)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Create paypal charge service exception", ex);
                throw;
            }
        }
    }
}
