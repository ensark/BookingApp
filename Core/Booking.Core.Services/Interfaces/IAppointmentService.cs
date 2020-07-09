using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Booking.Core.Domain.DTOs;
using Booking.Core.Domain.Queries;
using Booking.Common.Shared;

namespace Booking.Core.Services.Interfaces
{
    public interface IAppointmentService
    {
        IEnumerable<AppointmentDto> CreateAppointment(long userId, IEnumerable<AddAppointmentDto> addAppointmentDto);

        Task UpdateAppointmentTimeAsync(long id, long userId, UpdateAppointmentTimeDto updateAppointmentDto, CancellationToken cancellationToken);

        Task DeleteAppointmentAsync(long id, CancellationToken cancellationToken);

        Task<IEnumerable<AppointmentDto>> GetAppointmentsByReservationIdAsync(long reservationId, CancellationToken cancellationToken);

        Task<PagedResult<CustomerAppoinmtentsDto>> GetAppointmensByCustomerAsync(long userId, PagedQuery pagedQuery, CancellationToken cancellationToken);
    }
}
