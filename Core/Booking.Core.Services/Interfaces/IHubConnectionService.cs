using System.Threading;
using System.Threading.Tasks;

namespace Booking.Core.Services.Interfaces
{
    public interface IHubConnectionService
    {
        Task SaveHubConnectionAsync(long userId, string connectionId, CancellationToken cancellationToken);
    }
}
