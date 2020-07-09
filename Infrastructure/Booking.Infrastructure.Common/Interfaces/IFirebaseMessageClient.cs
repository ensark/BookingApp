using System.Threading;
using System.Threading.Tasks;
using Booking.Core.Domain.DTOs;

namespace Booking.Infrastructure.Common.Interfaces
{
    public interface IFirebaseMessageClient
    {
        Task SendPushNotificationAsync(PushNotificationRequestDto pushNotificationReques, CancellationToken cancellationToken);
    }
}
