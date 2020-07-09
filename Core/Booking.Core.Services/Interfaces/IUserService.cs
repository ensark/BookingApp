using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Booking.Core.Domain.DTOs;

namespace Booking.Core.Services.Interfaces
{
    public interface IUserService
    {
        Task UpdateUserPrivacySettingsAsync(long userId, UpdateUserPrivacySettingsDto updateUserPrivacySettings, CancellationToken cancellationToken);

        Task UpdateBiographyAsync(long id, JsonPatchDocument<PatchUserDto> patchDoc, CancellationToken cancellationToken);

        Task UpdateNotificationSettingsAsync(long userId, long notificationSettingsId, JsonPatchDocument<NotificationSettingsDto> patchDoc, CancellationToken cancellationToken);

        Task UpdateRemindersAsync(long userId, long reminderId, JsonPatchDocument<ReminderDto> patchDoc, CancellationToken cancellationToken);       

        Task DeleteUserAsync(long id, CancellationToken cancellationToken);

        Task<UserDto> GetUserByIdAsync(long id, CancellationToken cancellationToken);

        Task<ProviderProfileDto> GetProviderProfileAsync(long userId, CancellationToken cancellationToken);

        Task<CustomerProfileDto> GetCustomerProfileAsync(long userId, CancellationToken cancellationToken);
    }
}
