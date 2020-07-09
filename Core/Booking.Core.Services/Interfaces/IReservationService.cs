using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Booking.Core.Domain.DTOs;

namespace Booking.Core.Services.Interfaces
{
    public interface IReservationService
    {
        Task<ReservationDto> CreateReservationAsync(long userId, AddReservationDto addReservationDto, CancellationToken cancellationToken);

        Task ReservationRequestAsync(long reservationId, long userId, CancellationToken cancellationToken);

        Task ReservationRequestAnswerAsync(long reservationId, long userId, bool isAccepted, CancellationToken cancellationToken);

        Task<IEnumerable<ReservationRequestDto>> GetReservationRequestsAsync(long providerId, CancellationToken cancellationToken);

        Task<IEnumerable<string>> GetProviderAvailabiltyAsync(long providerId, DateTime requestDate, CancellationToken cancellationToken);       
    }
}
