using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Booking.Core.Domain.DTOs;

namespace Booking.Core.Services.Interfaces
{
    public interface IProviderSkillService
    {
        Task<ProviderSkillDto> CreateProviderSkillAsync(long userId, AddProviderSkillDto addProviderSkillDto, CancellationToken cancellationToken);

        Task UpdateProviderSkillAsync(long userId, long providerSkillId, AddProviderSkillDto addProviderSkillDto, CancellationToken cancellationToken);        

        Task DeleteProviderSkillAsync(long providerSkillId, CancellationToken cancellationToken);

        Task<IEnumerable<ProviderSkillDto>> GetProviderSkillsAsync(long userId, CancellationToken cancellationToken);
    }
}
