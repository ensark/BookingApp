using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Booking.Core.Domain.DTOs;

namespace Booking.Infrastructure.Common.Interfaces
{
    public interface IStripeCardPaymentService
    {
        Task<PaymentResponseDto> CreateChargeAsync(ProcessPaymentDto proccessCardPaymentDto, CancellationToken cancellationToken);

        Task<PaymentResponseDto> CreateCustomerAsync(ProcessPaymentDto proccessCardPaymentDto, CancellationToken cancellationToken);

        Task<IEnumerable<ProviderTransactionsDto>> GetChargesByCustomerAsync(string providerStripeId, CancellationToken cancellationToken);
    }
}
