using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Booking.Common.Shared;
using Booking.Core.Domain.DTOs;
using Booking.Core.Domain.Enums;
using Booking.Core.Domain.Queries;

namespace Booking.Core.Services.Interfaces
{
    public interface IProviderService
    {
        Task<ProviderDto> CreateProviderAsync(long userId, AddProviderDto addProviderDto, CancellationToken cancellationToken);

        Task UpdateProviderAsync(long id, long userId, AddProviderDto updateProviderDto, CancellationToken cancellationToken);

        Task DeleteProviderAsync(long id, CancellationToken cancellationToken);

        Task<IEnumerable<ProviderDto>> GetProvidersByServiceTypeAsync(long userId, ServiceType serviceType, CancellationToken cancellationToken);

        Task<PagedResult<ProviderSearchListDto>> GetAllProvidersAsync(SearchQuery searchQuery, CancellationToken cancellationToken);

        Task<ProviderDto> GetProviderByIdAsync(long id, CancellationToken cancellationToken);
        
        IList<string> CalculateTimeSlots(long userId, CalculateTimeSlotsValuesDto calculateTimeSlotsDto);
    }
}
