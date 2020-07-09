using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Booking.Common.Shared;
using Booking.Core.Domain.DTOs;
using Booking.Core.Domain.Queries;

namespace Booking.Core.Services.Interfaces
{
    public interface IConnectionService
    {
        Task<UserDto> CreateConnectionRequestAsync(long userId, long connectionUserId, CancellationToken cancellationToken);

        Task ConnectionRequestAnswerAsync(long connectionId, bool isAccepted, CancellationToken cancellationToken);

        Task<IEnumerable<AddConnectionDto>> GetConnectionRequestsAsync(long userId, CancellationToken cancellationToken);

        Task DeleteConnectionAsync(long connectionId, CancellationToken cancellationToken);

        Task<PagedResult<UserDto>> GetConnectonsAsync(long userId, SearchQuery searchQuery, CancellationToken cancellationToken);
    }
}
