using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Booking.Core.Domain.DTOs;

namespace Booking.Core.Services.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewDto> CreateReviewAsync(long userId, AddReviewDto addReviewDto, CancellationToken cancellationToken);

        Task<IEnumerable<ReviewDto>> GetReviewsAsync(long userId, CancellationToken cancellationToken);
    }
}
