using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Hangfire;
using Stripe;
using Booking.Infrastructure.Common.Interfaces;
using Booking.Infrastructure.Database;
using Booking.Core.Domain.DTOs;
using Booking.Core.Services.Interfaces;
using Booking.Core.Domain.Enums;
using Booking.Core.Services.Constants;

namespace Booking.Core.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly BookingDBContext _context;
        private readonly ILogger<PaymentService> _logger;
        private readonly IStripeCardPaymentService _stripeCardPaymentService;
        private readonly IPayPalCardPaymentService _payPalCardPaymentService;

        private readonly decimal _voucherCodeDiscount;

        public PaymentService(BookingDBContext context, IConfiguration configuration, ILogger<PaymentService> logger, IStripeCardPaymentService stripeCardPaymentService, IPayPalCardPaymentService payPalCardPaymentService)
        {
            _context = context;
            _logger = logger;
            _stripeCardPaymentService = stripeCardPaymentService;
            _payPalCardPaymentService = payPalCardPaymentService;

            _voucherCodeDiscount = Convert.ToDecimal(configuration[Config.VOUCHER_CODE_DISCOUNT]);
        }

        public async Task<ShowPriceDto> CalculateReservationPriceAsync(CalculatePriceDto calculatePriceDto, CancellationToken cancellationToken)
        {
            try
            {
                var reservation = await _context.Reservations.FirstOrDefaultAsync(x => x.Id == calculatePriceDto.ReservationId, cancellationToken);

                if (reservation is null)
                    throw new Exception($"Reservation with id {calculatePriceDto.ReservationId} not found.");

                decimal discountPrice = 0;
                decimal voucherDiscount = 0;

                if (!string.IsNullOrEmpty(calculatePriceDto.VoucherCode))
                {
                    voucherDiscount = await ValidateVoucherCode(calculatePriceDto.VoucherCode, cancellationToken) ? _voucherCodeDiscount : 0;
                }

                if (calculatePriceDto.PayTotal && (reservation.FiveSessionsDiscount > 0 || reservation.TenSessionsDiscount > 0))
                {
                    discountPrice = CalculatePriceWithDiscounts(reservation.TotalPrice, Convert.ToDecimal(reservation.FiveSessionsDiscount), Convert.ToDecimal(reservation.TenSessionsDiscount), voucherDiscount);
                }

                else if (calculatePriceDto.PayTotal)
                {
                    discountPrice = CalculatePriceWithDiscounts(reservation.TotalPrice, Convert.ToDecimal(reservation.FiveSessionsDiscount = 0), Convert.ToDecimal(reservation.TenSessionsDiscount = 0), voucherDiscount);
                }

                var totalPercent = Convert.ToDecimal(reservation.FiveSessionsDiscount) + Convert.ToDecimal(reservation.TenSessionsDiscount) + voucherDiscount;
                var pricePerSession = reservation.Appointments.Select(x => x.PricePerSession).FirstOrDefault();

                return new ShowPriceDto
                {
                    PricePerSession = reservation.Appointments.Select(x => x.PricePerSession).FirstOrDefault(),
                    DiscountPerSessionPrice = (pricePerSession - (pricePerSession * totalPercent / 100)),
                    TotalPrice = reservation.Appointments.Sum(x => x.PricePerSession),
                    DiscountTotalPrice = discountPrice,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Show price with discount service exception: ");
                throw;
            }
        }

        public async Task ProcessPaymentAsync(ProcessPaymentDto processPaymentDto, CancellationToken cancellationToken)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    processPaymentDto = await _context.Reservations.Where(x => x.Id == processPaymentDto.ReservationId && x.ReservationStatus != ReservationStatus.Paid)
                                                                   .Select(x => new ProcessPaymentDto
                                                                   {
                                                                       ProviderEmail = x.Provider.User.Email,
                                                                       CustomerName = $"{x.User.FirstName} {x.User.LastName }",
                                                                       CustomerEmail = x.User.Email,
                                                                       AmountPerSession = processPaymentDto.AmountPerSession,
                                                                       TotalAmount = processPaymentDto.TotalAmount,
                                                                       Description = $"Reservation ID: {x.ReservationPaymentId.ToString()} Title: {x.Provider.Title}",
                                                                       ProviderStripeId = x.Provider.ProviderStrypeId,
                                                                       Metadata = x.Appointments.ToDictionary(a => a.AppointmentExternalId.ToString(),
                                                                                                              a => $"Appointment: { a.AppointmentTime.ToString("dd MMMM yyyy HH:mm")}"),                                                                                                                                           
                                                                       ReservationId = processPaymentDto.ReservationId,
                                                                       PaymentProvider = processPaymentDto.PaymentProvider,
                                                                       PayPerSession = processPaymentDto.PayPerSession,
                                                                       PayTotal = processPaymentDto.PayTotal,
                                                                       VoucherCode = processPaymentDto.VoucherCode,
                                                                       CardHolder = processPaymentDto.CardHolder,
                                                                       CardNumber = processPaymentDto.CardNumber,
                                                                       ExpirationYear = processPaymentDto.ExpirationYear,
                                                                       ExpirationMonth = processPaymentDto.ExpirationMonth,
                                                                       Cvc = processPaymentDto.Cvc                                                                      
                                                                   })
                                                                   .FirstOrDefaultAsync(cancellationToken);

                    if (processPaymentDto is null)
                         throw new Exception($"Reservation did not found.");

                    _logger.LogInformation("Payment processing ...");

                    var processingPaymentResponse = new PaymentResponseDto();

                    var reservation = await _context.Reservations.FirstOrDefaultAsync(x => x.Id == processPaymentDto.ReservationId, cancellationToken);

                    reservation.PaymentProvider = processPaymentDto.PaymentProvider;
                    reservation.PayPerSession = processPaymentDto.PayPerSession;
                    reservation.PayTotal = processPaymentDto.PayTotal;
                    reservation.TotalPriceDiscount = processPaymentDto.TotalAmount;
                    reservation.VoucherCode = await ValidateVoucherCode(processPaymentDto.VoucherCode, cancellationToken) ? processPaymentDto.VoucherCode : null;
                    reservation.VoucherCodeDiscount = await ValidateVoucherCode(processPaymentDto.VoucherCode, cancellationToken) ? Convert.ToInt32(_voucherCodeDiscount): 0;
                                       
                    await _context.SaveChangesAsync(cancellationToken);

                    if (processPaymentDto.PayTotal)
                    {
                        processingPaymentResponse = await ProccessTotalPaymentAsync(processPaymentDto, cancellationToken);
                    }

                    else if (processPaymentDto.PayPerSession)
                    {
                        processingPaymentResponse = await PreparePaymentPayPerSessionAsync(processPaymentDto, cancellationToken);                                              
                    }

                    if (processingPaymentResponse.Status == HttpStatusCode.OK || processingPaymentResponse.Status == HttpStatusCode.Created)
                    {
                        reservation.Provider.ProviderStrypeId = processPaymentDto.PaymentProvider == PaymentProvider.Stripe ? processingPaymentResponse.ProviderStrypeId : "";
                        reservation.ReservationStatus = processPaymentDto.PayTotal ? ReservationStatus.Paid : ReservationStatus.InProgress;
                        reservation.PayPerSession = processPaymentDto.PayPerSession ? true : false;
                        reservation.PayTotal = processPaymentDto.PayTotal ? true: false ;

                        foreach (var amount in reservation.Appointments)
                        {
                            amount.PricePerSessionDiscount = processPaymentDto.AmountPerSession;
                            amount.PayPalApprovalLink = processingPaymentResponse.PayPalApprovalLink;
                        }

                        await ResetVoucherCode(processPaymentDto.VoucherCode, cancellationToken);

                        await _context.SaveChangesAsync(cancellationToken);

                        transaction.Commit();

                        _logger.LogInformation("Payment was successfully processed");
                    }

                    else
                        _logger.LogError("Payment was not processed");

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError(ex, "Proccess total payment service exception: ");
                    throw;
                }
            }
        }

        public async Task ProcessPaymentPerSessionAsync(IJobCancellationToken cancellationToken)
        {
            try
            {
                var paymentsPerSession = await _context.Appointments
                                                             .Include(x => x.Reservation)
                                                             .ThenInclude(x => x.Provider)
                                                             .ThenInclude(x => x.User)
                                                             .Where(x => x.Reservation.PayPerSession
                                                             &&
                                                                                        DateTime.Now > x.AppointmentTime
                                                             &&
                                                                                        x.AppointmentStatus == AppointmentStatus.Scheduled
                                                             &&
                                                                                        x.Reservation.ReservationStatus == ReservationStatus.InProgress)
                                                             .Select(x => new ProcessPaymentDto
                                                             {
                                                                 AmountPerSession = Convert.ToInt64(x.Reservation.Appointments.Select(a => a.PricePerSessionDiscount).FirstOrDefault()),
                                                                 ProviderEmail = x.Reservation.Provider.User.Email,
                                                                 Description = x.Reservation.Provider.Title,
                                                                 ProviderStripeId = x.Reservation.Provider.ProviderStrypeId,
                                                                 ReservationId = x.ReservationId,
                                                             })
                                                             .ToListAsync(cancellationToken.ShutdownToken);

                if (paymentsPerSession.Any())
                {
                    foreach (var processPayment in paymentsPerSession)
                    {
                        var processingCardStatus = new PaymentResponseDto();

                        if (processPayment.PaymentProvider == PaymentProvider.Stripe)
                        {
                            processingCardStatus = await _stripeCardPaymentService.CreateChargeAsync(processPayment, cancellationToken.ShutdownToken);
                        }

                        else if (processPayment.PaymentProvider == PaymentProvider.PayPal)
                        {
                            processingCardStatus = await _payPalCardPaymentService.CreateChargeAsync(processPayment, cancellationToken.ShutdownToken);
                        }

                        if (processingCardStatus.Status == HttpStatusCode.OK)
                        {
                            var appointment = await _context.Appointments.Where(x => x.Reservation.Id == processPayment.ReservationId)
                                                                         .ToListAsync(cancellationToken.ShutdownToken);

                            foreach (var item in appointment)
                            {
                                item.AppointmentStatus = AppointmentStatus.Completed;
                                await _context.SaveChangesAsync(cancellationToken.ShutdownToken);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Proccess payment per session service exception: ");
                throw;
            }
        }

        public async Task CheckPaymentStatusAsync(Event stripeEvent, CancellationToken cancellationToken)
        {
            try
            {
                var charge = (Charge)stripeEvent.Data.Object;

                var appointmentStripeIds = new List<string>(charge.Metadata.Keys);
                var appointments = await _context.Appointments.Where(a => appointmentStripeIds.Contains(a.AppointmentExternalId.ToString())).ToListAsync(cancellationToken);

                foreach (var appointment in appointments)
                {
                    switch (charge.Status)
                    {
                        case "pending":
                            appointment.AppointmentStatus = AppointmentStatus.NotPaid;
                            break;
                        case "succeeded":
                            appointment.AppointmentStatus = AppointmentStatus.Paid;
                            break;
                        case "failed":
                            appointment.AppointmentStatus = AppointmentStatus.Failed;
                            break;
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Check payment status service exception: ");
                throw;
            }
        }

        public async Task<IEnumerable<ProviderTransactionsDto>> GetTransactionsByProvider(long userId, CancellationToken cancellationToken)
        {
            try
            {
                var providerStripeIds = await _context.Providers.Where(x => x.UserId == userId).Select(x => x.ProviderStrypeId).ToListAsync(cancellationToken);

                if (!providerStripeIds.Any())
                    throw new Exception("Provider stripe id is null");

                foreach (var providerStripeId in providerStripeIds)
                {
                    return await _stripeCardPaymentService.GetChargesByCustomerAsync(providerStripeId, cancellationToken);
                }
                return Array.Empty<ProviderTransactionsDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get transactions by provider service exception: ");
                throw;
            }
        }

        private async Task<PaymentResponseDto> ProccessTotalPaymentAsync(ProcessPaymentDto processPaymentDto, CancellationToken cancellationToken)
        {
            try
            {
                var paymentResponse = new PaymentResponseDto();

                if (processPaymentDto.PaymentProvider == PaymentProvider.Stripe)
                {
                    paymentResponse = await _stripeCardPaymentService.CreateChargeAsync(processPaymentDto, cancellationToken);
                }
                else if (processPaymentDto.PaymentProvider == PaymentProvider.PayPal)
                {
                    paymentResponse = await _payPalCardPaymentService.CreateChargeAsync(processPaymentDto, cancellationToken);
                }

                return paymentResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Proccess total payment service exception: ");
                throw;
            }
        }

        private async Task<PaymentResponseDto> PreparePaymentPayPerSessionAsync(ProcessPaymentDto processPaymentDto, CancellationToken cancellationToken)
        {
            try
            {
                var paymentResponse = new PaymentResponseDto();

                if (processPaymentDto.PaymentProvider == PaymentProvider.Stripe)
                {
                    paymentResponse = await _stripeCardPaymentService.CreateCustomerAsync(processPaymentDto, cancellationToken);
                }
                else if (processPaymentDto.PaymentProvider == PaymentProvider.PayPal)
                {                   
                    paymentResponse.Status = HttpStatusCode.OK;
                }

                return paymentResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create credit card customer service exception: ");
                throw;
            }
        }

        private async Task<bool> ValidateVoucherCode(string voucherCode, CancellationToken cancellationToken)
        {
            try
            {
                var iSValidCode = true;

                if (!string.IsNullOrEmpty(voucherCode))
                {
                    var code = await _context.VoucherCodes.SingleOrDefaultAsync(x => x.Code == voucherCode && !x.IsUsed, cancellationToken);

                    if (code is null)
                        throw new Exception($"Voucher code {code} is not valid");
                }

                return await Task.FromResult(iSValidCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Validate voucher service exception: ");
                throw;
            }
        }

        private async Task ResetVoucherCode(string voucherCode, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(voucherCode))
            {
                var code = await _context.VoucherCodes.FirstOrDefaultAsync(x => x.Code == voucherCode, cancellationToken);

                if (code is null)
                    throw new Exception($"Voucher code {voucherCode} was not reset.");

                code.IsUsed = true;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        private decimal CalculatePriceWithDiscounts(decimal totalPrice, decimal fiveSessionsDiscount, decimal thenSessionsDiscount, decimal voucherCodeDiscount)
        {
            var sumOfDiscounts = fiveSessionsDiscount + thenSessionsDiscount + voucherCodeDiscount;
            decimal discount;
            decimal discountPrice;

            if (sumOfDiscounts > 0)
            {
                discount = (totalPrice * sumOfDiscounts) / 100;
                discountPrice = totalPrice - discount;
            }

            else
                discountPrice = totalPrice;

            return discountPrice;
        }
    }
}
