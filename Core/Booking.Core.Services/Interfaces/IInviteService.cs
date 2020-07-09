using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;

namespace Booking.Core.Services.Interfaces
{
    public interface IInviteService
    {
        Task SendInvitesAsync(long userId, IEnumerable<string> phoneNumbers, CancellationToken cancellationToken);

        Task CheckAcceptedInvitesAsync(IJobCancellationToken cancellationToken);
    }
}
