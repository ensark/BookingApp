using System.Threading;
using System.Threading.Tasks;
using Booking.Core.Domain.DTOs;

namespace Booking.Core.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserDto> RegisterUserAsync(RegisterUserDto registerUserDto, CancellationToken cancellationToken);

        Task<LoggedUserDto> AuthenticateUserAsync(AuthenticateUserDto authenticateUserDto, CancellationToken cancellationToken);

        Task ChangePasswordAsync(long userId, string newPassword, CancellationToken cancellationToken);

        Task<TokenResponseDto> RefreshAccessToken(string token, CancellationToken cancellationToken);

        Task<LoggedUserDto> BecomeServiceProviderAsync(long userId, CancellationToken cancellationToken);

        Task ForgotPasswordAsync(string phoneNumber, CancellationToken cancellationToken);
    }
}
