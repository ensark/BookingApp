using System.Threading;
using System.Threading.Tasks;
using Booking.Common.Shared;
using Booking.Core.Domain.DTOs;
using Booking.Core.Domain.Queries;

namespace Booking.Core.Services.Interfaces
{
    public interface IPushNotificationService
    {
        Task SavePushNotificationAsync(AddNotificationDto addNotificationDto, CancellationToken cancellationToken);

        Task<PagedResult<NotificationDto>> GetPushNotificationsAsync(long userId, PagedQuery pagedQuery, CancellationToken cancellationToken);
    }
}
