using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Stripe;
using Booking.Core.Domain.DTOs;
using Booking.Infrastructure.Common.Interfaces;

namespace Booking.Infrastructure.Payments.Stripe
{
    public class StripeCardPaymentService : IStripeCardPaymentService
    {
        private const string CURRRENCY_GBP = "GBP";
        private const bool CAPTURE_CARD_PAYMENT = true;

        private readonly ILogger<StripeCardPaymentService> _logger;
        private readonly ChargeService _chargeService;
        private readonly CustomerService _customerService;

        public StripeCardPaymentService(ILogger<StripeCardPaymentService> logger)
        {
            _logger = logger;
            _chargeService = new ChargeService();
            _customerService = new CustomerService();
        }

        public async Task<PaymentResponseDto> CreateChargeAsync(ProcessPaymentDto proccessCardPaymentDto, CancellationToken cancellationToken)
        {
            try
            {
                var provider = new CustomerCreateOptions();
                var stripeCustomer = new Customer();
                var existStripeCustomer = new Customer();

                if (string.IsNullOrEmpty(proccessCardPaymentDto.ProviderStripeId))
                {
                    // Testing Card without frontend and token

                    var card = new CreditCardOptions()
                    {
                        Name = proccessCardPaymentDto.CardHolder,
                        Number = proccessCardPaymentDto.CardNumber,
                        ExpMonth = proccessCardPaymentDto.ExpirationMonth,
                        ExpYear = proccessCardPaymentDto.ExpirationYear,
                        Cvc = proccessCardPaymentDto.Cvc
                    };

                    var token = new TokenCreateOptions { Card = card };

                    var serviceToken = new TokenService();
                    var newToken = await serviceToken.CreateAsync(token, null, cancellationToken);

                    // Testing Card without frontend and token

                    provider.Email = proccessCardPaymentDto.ProviderEmail;
                    provider.Name = proccessCardPaymentDto.CustomerName;
                    provider.Source = newToken.Id; // newToken.Id REPLACE WTIH proccessCardPaymentDto.Token WHEN BACKEND CONNECTED WITH FRONTEND 
                }
                else
                {
                    existStripeCustomer = await _customerService.GetAsync(proccessCardPaymentDto.ProviderStripeId, null, null, cancellationToken);
                }

                stripeCustomer = string.IsNullOrEmpty(existStripeCustomer.Email) ? await _customerService.CreateAsync(provider, null, cancellationToken) : existStripeCustomer;

                var options = new ChargeCreateOptions
                {
                    TransferGroup = Guid.NewGuid().ToString(),
                    Currency = CURRRENCY_GBP,
                    Capture = CAPTURE_CARD_PAYMENT,
                    Customer = stripeCustomer.Id,
                    Amount = Convert.ToInt64(proccessCardPaymentDto.TotalAmount) * 100,
                    ReceiptEmail = proccessCardPaymentDto.ProviderEmail,
                    Description = proccessCardPaymentDto.Description,
                    Metadata = proccessCardPaymentDto.Metadata
                };

                var charge = await _chargeService.CreateAsync(options);

                return new PaymentResponseDto
                {
                    Status = charge.StripeResponse.StatusCode,
                    ProviderStrypeId = stripeCustomer.Id,
                    Created = charge.Created
                };
            }

            catch (StripeException ex)
            {
                _logger.LogError("Stripe service exception", ex.StripeError.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("Create charge service exception", ex);
                throw;
            }
        }

        public async Task<PaymentResponseDto> CreateCustomerAsync(ProcessPaymentDto proccessCardPaymentDto, CancellationToken cancellationToken)
        {
            try
            {
                var provider = new CustomerCreateOptions();
                var stripeCustomer = new Customer();

                if (string.IsNullOrEmpty(proccessCardPaymentDto.ProviderStripeId))
                {
                    // Testing Card without frontend and token

                    var card = new CreditCardOptions()
                    {
                        Name = proccessCardPaymentDto.CardHolder,
                        Number = proccessCardPaymentDto.CardNumber,
                        ExpMonth = proccessCardPaymentDto.ExpirationMonth,
                        ExpYear = proccessCardPaymentDto.ExpirationYear,
                        Cvc = proccessCardPaymentDto.Cvc
                    };

                    var token = new TokenCreateOptions { Card = card };

                    var serviceToken = new TokenService();
                    var newToken = await serviceToken.CreateAsync(token, null, cancellationToken);

                    // Testing Card without frontend and token

                    provider.Email = proccessCardPaymentDto.ProviderEmail;
                    provider.Name = proccessCardPaymentDto.CustomerName;
                    provider.Source = newToken.Id;
                    // newToken.Id REPLACE WTIH proccessCardPaymentDto.Token WHEN BACKEND CONNECTED WITH FRONTEND 
                }

                var existStripeCustomer = await _customerService.GetAsync(proccessCardPaymentDto.ProviderStripeId, null, null, cancellationToken);

                stripeCustomer = string.IsNullOrEmpty(existStripeCustomer.Email) ? await _customerService.CreateAsync(provider, null, cancellationToken) : existStripeCustomer;

                return new PaymentResponseDto
                {
                    Status = stripeCustomer.StripeResponse.StatusCode,
                    ProviderStrypeId = stripeCustomer.Id,
                    Created = stripeCustomer.Created
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Create customer service exception", ex);
                throw;
            }
        }

        public async Task<IEnumerable<ProviderTransactionsDto>> GetChargesByCustomerAsync(string providerStripeId, CancellationToken cancellationToken)
        {
            try
            {
                var options = new ChargeListOptions
                {
                    Limit = 100,
                    Customer = providerStripeId
                };

                var chargesByCustomer = await _chargeService.ListAsync(options, null, cancellationToken);

                var chargeItems = new List<ProviderTransactionsDto>();

                foreach (var charge in chargesByCustomer.Data)
                {
                    var item = new ProviderTransactionsDto
                    {
                        Customer = charge.BillingDetails.Name,
                        Amount = charge.Amount / 100,
                        Created = charge.Created,
                    };
                    chargeItems.Add(item);
                }

                return chargeItems;
            }
            catch (Exception ex)
            {
                _logger.LogError("Create credit card customer service exception", ex);
                throw;
            }
        }        
    }
}
