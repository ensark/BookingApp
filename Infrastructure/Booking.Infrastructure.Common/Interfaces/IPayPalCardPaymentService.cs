using System.Threading;
using System.Threading.Tasks;
using PayPal.v1.Payments;
using Booking.Core.Domain.DTOs;

namespace Booking.Infrastructure.Common.Interfaces
{
    public interface IPayPalCardPaymentService
    {
        Task<PaymentResponseDto> CreateChargeAsync(ProcessPaymentDto proccessCardPaymentDto, CancellationToken cancellationToken);
    }
}
