using System;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Booking.Core.Services.Interfaces;
using Booking.Core.Domain.DTOs;
using Booking.Infrastructure.Database;
using Booking.Core.Domain.Entities;
using Booking.Core.Domain.Enums;
using Booking.Core.Services.Constants;
using Booking.Common.Exceptions;
using Booking.Infrastructure.Common.Interfaces;

namespace Booking.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly BookingDBContext _context;
        private readonly ILogger<AuthService> _logger;
        private readonly ISmsService _smsService;
        private readonly string _jwtSecretKey;
        private readonly double _accessTokenExpire;

        private static readonly ThreadLocal<Random> _random = new ThreadLocal<Random>(() => new Random());

        public AuthService(BookingDBContext context, IConfiguration configuration, ILogger<AuthService> logger, ISmsService smsService)
        {
            _context = context;
            _jwtSecretKey = configuration[Config.JWT_SECRET_API_KEY];
            _accessTokenExpire = Convert.ToDouble(configuration[Config.JWT_ACCESS_TOKEN_EXPIRE]);
            _logger = logger;
            _smsService = smsService;
        }

        public async Task<UserDto> RegisterUserAsync(RegisterUserDto registerUserDto, CancellationToken cancellationToken)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    UserDto userDto = null;

                    if (registerUserDto != null)
                    {
                        byte[] passwordHash, passwordSalt;
                        CreatePasswordHash(registerUserDto.Password, out passwordHash, out passwordSalt);

                        var user = new User
                        {
                            FirstName = registerUserDto.FirstName,
                            LastName = registerUserDto.LastName,
                            Email = registerUserDto.Email,
                            Phone = registerUserDto.Phone,
                            UserType = registerUserDto.UserType,
                            FcmTokenDeviceId = registerUserDto.FcmTokenDeviceId,
                            Role = MapUserTypeToRole(registerUserDto.UserType),
                            PasswordHash = passwordHash,
                            PasswordSalt = passwordSalt,
                            Address = new Address
                            {
                                Street = registerUserDto.Address.Street,
                                City = registerUserDto.Address.City,
                                Postcode = registerUserDto.Address.Postcode,
                                Country = registerUserDto.Address.Country
                            },
                        };

                        _logger.LogInformation($"Register user: {user.FirstName}''{user.LastName}");

                        _context.Users.Add(user);
                        await _context.SaveChangesAsync(cancellationToken);

                        user.CreatedBy = user.Id.ToString();
                        user.Address.CreatedBy = user.Id.ToString();

                        _context.NotificationSettings.Add(new NotificationSettings
                        {
                            CreatedBy = user.Id.ToString(),
                            UserId = user.Id,
                            BookingConfirmations = false,
                            RecommendationRequestFromFriends = false,
                            PrivateMessages = false,
                            NewBookings = false,
                            AutomaticBookingConfirmation = false,
                            NotificationSettingsType = user.Role == Role.ServiceProvider ? NotificationSettingsType.Provider : NotificationSettingsType.Customer
                        });

                        _context.Reminders.Add(new Reminder
                        {
                            CreatedBy = user.Id.ToString(),
                            UserId = user.Id,
                            Booking24HoursBefore = false,
                            Booking1HourBefore = false,
                            Booking15MinutesBefore = false
                        });

                        await _context.SaveChangesAsync(cancellationToken);

                        transaction.Commit();

                        userDto = new UserDto
                        {
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            Phone = user.Phone,
                            Street = user.Address.Street,
                            City = user.Address.City,
                            Postcode = user.Address.Postcode,
                            Country = user.Address.Country,
                        };
                    }

                    return userDto;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError(ex, "Create user service exception:");
                    throw;
                }
            }
        }

        public async Task<LoggedUserDto> AuthenticateUserAsync(AuthenticateUserDto authenticateUserDto, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == authenticateUserDto.Email, cancellationToken);

                LoggedUserDto loggedUserDto = null;

                if (user != null)
                {
                    var currentRefreshTokens = await _context.RefreshTokens.Where(x => x.UserId == user.Id).ToListAsync(cancellationToken);

                    _context.RefreshTokens.RemoveRange(currentRefreshTokens);
                    await _context.SaveChangesAsync(cancellationToken);

                    var tokenData = GenerateToken(user.Id, user.Role);
                    var refreshToken = await AddRefreshTokenAsync(user.Id, user.Role, cancellationToken);

                    var loggedUser = await LoggedUserData(user.Id, cancellationToken);

                    loggedUserDto = new LoggedUserDto
                    {
                        FirstName = loggedUser.FirstName,
                        LastName = loggedUser.LastName,
                        City = loggedUser.City,
                        Title = loggedUser.Title,
                        Rank = loggedUser.Rank,
                        ProfileImage = loggedUser.ProfileImage,
                        AccessToken = tokenData.AccessToken,
                        AccessTokenExpiration = tokenData.AccessTokenExpiration,
                        RefreshToken = refreshToken.Token
                    };
                }

                return loggedUserDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Authenticate user service exception:");
                throw;
            }
        }

        public async Task ChangePasswordAsync(long userId, string newPassword, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

                if (user is null)
                    throw new NotFoundException($"User doesn't exist.");

                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(newPassword, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Authenticate user service exception:");
                throw;
            }
        }

        public async Task ForgotPasswordAsync(string phoneNumber, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Phone == phoneNumber.Trim(), cancellationToken);

                if (user is null)
                    throw new NotFoundException($"User with number {phoneNumber} doesn't exist.");

                var newPassword = GenerateCode();

                await ChangePasswordAsync(user.Id, newPassword, cancellationToken);

                var sendSms = new SmsDto
                {
                    Message = $"Dear {user.FirstName} {user.LastName} your new Password is: {newPassword}",
                    ReceiverNumber = user.Phone
                };

                var sendSmsResponse = await _smsService.SendSmsAsync(sendSms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Forgot password service exception:");
                throw;
            }
        }

        public async Task<LoggedUserDto> BecomeServiceProviderAsync(long userId, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

                if (user is null)
                    throw new NotFoundException($"User doesn't exist.");

                user.ModifiedBy = userId.ToString();
                user.ModifiedDate = DateTime.UtcNow;
                user.Role = Role.ServiceProvider;

                await _context.SaveChangesAsync(cancellationToken);

                var tokenData = GenerateToken(user.Id, user.Role);
                var refreshToken = await AddRefreshTokenAsync(user.Id, user.Role, cancellationToken);

                var loggedUser = await LoggedUserData(user.Id, cancellationToken);

                var loggedUserDto = new LoggedUserDto
                {
                    FirstName = loggedUser.FirstName,
                    LastName = loggedUser.LastName,
                    City = loggedUser.City,
                    Title = loggedUser.Title,
                    Rank = loggedUser.Rank,
                    ProfileImage = loggedUser.ProfileImage,
                    AccessToken = tokenData.AccessToken,
                    AccessTokenExpiration = tokenData.AccessTokenExpiration,
                    RefreshToken = refreshToken.Token
                };

                return loggedUserDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Become service a provider exception:");
                throw;
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(password))
                    throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

                using (var hmac = new HMACSHA512())
                {
                    passwordSalt = hmac.Key;
                    passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create password hash auth service exception:");
                throw;
            }
        }

        private TokenResponseDto GenerateToken(long userId, string role)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSecretKey);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userId.ToString()),
                    new Claim(ClaimTypes.Role, role)
                }),
                    Expires = DateTime.Now.AddMinutes(_accessTokenExpire),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var createToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(createToken);
                var expires = new JwtSecurityTokenHandler().ReadToken(token).ValidTo.ToLocalTime();

                return new TokenResponseDto
                {
                    AccessToken = token,
                    AccessTokenExpiration = expires
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Generate token service exception:");
                throw;
            }
        }

        private async Task<RefreshTokenDto> AddRefreshTokenAsync(long userId, string role, CancellationToken cancellationToken)
        {
            try
            {
                var refreshToken = new RefreshToken()
                {
                    CreatedBy = userId.ToString(),
                    Token = GenerateRefreshToken(),
                    UserId = userId,
                    Role = role
                };

                _context.Add(refreshToken);
                await _context.SaveChangesAsync(cancellationToken);

                return new RefreshTokenDto
                {
                    Token = refreshToken.Token,
                    UserId = refreshToken.UserId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Add refresh token service exception:");
                throw;
            }
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<TokenResponseDto> RefreshAccessToken(string token, CancellationToken cancellationToken)
        {
            try
            {
                var refreshToken = await GetRefreshAccessTokenAsync(token, cancellationToken);

                if (refreshToken is null)
                    new NotFoundException("Refresh token was not found.");

                var tokenRespone = GenerateToken(refreshToken.UserId, refreshToken.Role);
                tokenRespone.RefreshToken = refreshToken.Token;

                return tokenRespone;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Refresh access token service exception:");
                throw;
            }
        }

        private async Task<RefreshTokenDto> GetRefreshAccessTokenAsync(string token, CancellationToken cancellationToken)
        {
            var refreshToken = await _context.RefreshTokens.Where(x => x.Token == token)
                                               .Select(x => new RefreshTokenDto
                                               {
                                                   Token = x.Token,
                                                   UserId = x.UserId,
                                                   Role = x.Role
                                               })
                                              .FirstOrDefaultAsync(cancellationToken);

            if (refreshToken is null)
                throw new NotFoundException("Refresh token is not found!");

            return refreshToken;
        }

        private async Task<LoggedUserDto> LoggedUserData(long userId, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users.Include(x => x.Attachments)
                                               .Include(x => x.Providers)
                                               .Include(x => x.Reviews)
                                               .Where(x => x.Id == userId)
                                               .Select(x => new LoggedUserDto
                                               {
                                                   FirstName = x.FirstName,
                                                   LastName = x.LastName,
                                                   City = $"{x.Address.City} {x.Address.Country}",
                                                   Title = x.UserType == UserType.ServiceProvider ? x.Providers.Select(a => a.Title).FirstOrDefault() : "",
                                                   ProfileImage = x.Attachments.Any() ? x.Attachments.Where(a => a.DocumentType == DocumentType.ProfileImage).Select(a => a.Data).FirstOrDefault() : null,
                                                   Rank = x.Reviews.Any() ? decimal.Round(x.Reviews.Sum(a => a.Grade) / x.Reviews.Count, 2, MidpointRounding.AwayFromZero) : 0
                                               })
                                               .FirstOrDefaultAsync(cancellationToken);

                if (user is null)
                    throw new NotFoundException($"User with id {userId} does not exist.");

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get logged user data service exception:");
                throw;
            }
        }

        private string MapUserTypeToRole(UserType userType)
        {
            switch (userType)
            {
                case UserType.Administrator:
                    return Role.Admin;
                case UserType.Customer:
                    return Role.Customer;
                case UserType.ServiceProvider:
                    return Role.ServiceProvider;
                case UserType.CustomerAndServiceProvider:
                    return Role.CustomerAndServiceProvider;
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        private static string GenerateCode()
        {
            char[] chars = "ACDEFGHJKLMNPQRTUVWXYZabcdgfh234679!#$%".ToCharArray();

            var sb = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                int num = _random.Value.Next(0, chars.Length);
                sb.Append(chars[num]);
            }
            return sb.ToString();
        }
    }
}

