using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Hangfire;
using Stripe;
using Booking.Core.Domain.DTOs;

namespace Booking.Core.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<ShowPriceDto> CalculateReservationPriceAsync(CalculatePriceDto calculatePriceDto, CancellationToken cancellationToken);

        Task ProcessPaymentAsync(ProcessPaymentDto processPaymentDto, CancellationToken cancellationToken);

        Task ProcessPaymentPerSessionAsync(IJobCancellationToken cancellationToken);
      
        Task CheckPaymentStatusAsync(Event stripeEvent, CancellationToken cancellationToken);

        Task<IEnumerable<ProviderTransactionsDto>> GetTransactionsByProvider(long userId, CancellationToken cancellationToken);        
    }
}
