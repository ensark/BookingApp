using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.JsonPatch;
using Booking.Common.Exceptions;
using Booking.Core.Domain.DTOs;
using Booking.Core.Services.Interfaces;
using Booking.Infrastructure.Database;
using Booking.Core.Domain.Enums;

namespace Booking.Core.Services
{
    public class UserService : IUserService
    {
        private readonly BookingDBContext _context;
        private readonly ILogger<UserService> _logger;
        private readonly IReviewService _reviewService;        

        public UserService(BookingDBContext context, ILogger<UserService> logger, IReviewService reviewService)
        {
            _context = context;
            _logger = logger;
            _reviewService = reviewService;            
        }

        public async Task UpdateUserPrivacySettingsAsync(long userId, UpdateUserPrivacySettingsDto updateUserPrivacySettings, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

                if (user is null)
                    throw new NotFoundException($"User with id {userId} doesn't exist.");
               
                user.ModifiedBy = userId.ToString();
                user.ModifiedDate = DateTime.UtcNow;
                user.FirstName = !string.IsNullOrWhiteSpace(updateUserPrivacySettings.FirstName) ? updateUserPrivacySettings.FirstName: user.FirstName;
                user.LastName = !string.IsNullOrWhiteSpace(updateUserPrivacySettings.LastName) ? updateUserPrivacySettings.LastName : user.LastName;                
                user.Phone = !string.IsNullOrWhiteSpace(updateUserPrivacySettings.Phone) ? updateUserPrivacySettings.Phone : user.Phone;
                user.Address.Street = !string.IsNullOrWhiteSpace(updateUserPrivacySettings.Street) ? updateUserPrivacySettings.Street : user.Address.Street;
                user.Address.City = !string.IsNullOrWhiteSpace(updateUserPrivacySettings.City) ? updateUserPrivacySettings.City : user.Address.City;
                user.Address.Postcode = !string.IsNullOrWhiteSpace(updateUserPrivacySettings.Postcode) ? updateUserPrivacySettings.Postcode : user.Address.Postcode;
                user.Address.Country = !string.IsNullOrWhiteSpace(updateUserPrivacySettings.Country) ? updateUserPrivacySettings.Country : user.Address.Country;

                _logger.LogInformation($"Update user: {user}");

                _context.Users.Update(user);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update user address exception:");
                throw;
            }
        }

        public async Task UpdateBiographyAsync(long id, JsonPatchDocument<PatchUserDto> patchDoc, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

                if (user is null)
                    throw new NotFoundException($"User with id {id} doesn't exist.");

                var userDto = new PatchUserDto
                {
                    Biography = user.Biography,
                };

                patchDoc.ApplyTo(userDto);

                user.ModifiedBy = id.ToString();
                user.ModifiedDate = DateTime.UtcNow;
                user.Biography = userDto.Biography;

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update user biography exception:");
                throw;
            }
        }

        public async Task UpdateNotificationSettingsAsync(long userId, long notificationSettingsId, JsonPatchDocument<NotificationSettingsDto> patchDoc, CancellationToken cancellationToken)
        {
            try
            {
                var notificationSettings = await _context.NotificationSettings.FirstOrDefaultAsync(x => x.Id == notificationSettingsId, cancellationToken);

                if (notificationSettings is null)
                    throw new NotFoundException($"Notification settings with id {notificationSettingsId} doesn't exist.");

                var notificationSettingsDto = new NotificationSettingsDto
                {
                    BookingConfirmations = notificationSettings.BookingConfirmations,
                    RecommendationRequestFromFriends = notificationSettings.RecommendationRequestFromFriends,
                    PrivateMessages = notificationSettings.PrivateMessages,
                    NewBookings = notificationSettings.NewBookings,
                    AutomaticBookingConfirmation = notificationSettings.AutomaticBookingConfirmation
                };

                patchDoc.ApplyTo(notificationSettingsDto);

                notificationSettings.ModifiedBy = userId.ToString();
                notificationSettings.ModifiedDate = DateTime.UtcNow;
                notificationSettings.BookingConfirmations = notificationSettingsDto.BookingConfirmations;
                notificationSettings.RecommendationRequestFromFriends = notificationSettingsDto.RecommendationRequestFromFriends;
                notificationSettings.PrivateMessages = notificationSettingsDto.PrivateMessages;
                notificationSettings.NewBookings = notificationSettingsDto.NewBookings;
                notificationSettings.AutomaticBookingConfirmation = notificationSettingsDto.AutomaticBookingConfirmation;

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update notification settings exception:");
                throw;
            }
        }

        public async Task UpdateRemindersAsync(long userId, long reminderId, JsonPatchDocument<ReminderDto> patchDoc, CancellationToken cancellationToken)
        {
            try
            {
                var reminder = await _context.Reminders.FirstOrDefaultAsync(x => x.Id == reminderId, cancellationToken);

                if (reminder is null)
                    throw new NotFoundException($"Reminder with id {reminderId} doesn't exist.");

                var reminderDto = new ReminderDto
                {
                    Booking24HoursBefore = reminder.Booking24HoursBefore,
                    Booking1HourBefore = reminder.Booking1HourBefore,
                    Booking15MinutesBefore = reminder.Booking15MinutesBefore
                };

                patchDoc.ApplyTo(reminderDto);

                reminder.ModifiedBy = userId.ToString();
                reminder.ModifiedDate = DateTime.UtcNow;
                reminder.Booking24HoursBefore = reminderDto.Booking24HoursBefore;
                reminder.Booking1HourBefore = reminderDto.Booking1HourBefore;
                reminder.Booking15MinutesBefore = reminderDto.Booking15MinutesBefore;

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update reminder exception:");
                throw;
            }
        }
        
        public async Task DeleteUserAsync(long id, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

                if (user is null)
                    throw new NotFoundException($"User with id {id} doesn't exist.");

                _context.Users.Remove(user);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete user service exception:");
                throw;
            }
        }

        public async Task<UserDto> GetUserByIdAsync(long id, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users.Include(x => x.Address).Where(x => x.Id == id)
                                               .Select(x => new UserDto
                                               {
                                                   FirstName = x.FirstName,
                                                   LastName = x.LastName,
                                                   Email = x.Email,
                                                   Phone = x.Phone,
                                                   Street = x.Address.Street,
                                                   City = x.Address.City,
                                                   Postcode = x.Address.Postcode,
                                                   Country = x.Address.Country
                                               })
                                                .FirstOrDefaultAsync(cancellationToken);
                if (user is null)
                    throw new NotFoundException($"User with id {id} does not exist.");

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get user service exception:");
                throw;
            }
        }

        public async Task<ProviderProfileDto> GetProviderProfileAsync(long userId, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users.Include(x => x.Attachments)
                                               .Include(x => x.Reviews)
                                               .Include(x => x.ProviderSkills)
                                               .Where(x => x.Id == userId && x.Attachments.Any(a => a.DocumentType != DocumentType.ProfileImage))
                                               .Select(x => new ProviderProfileDto
                                               {
                                                   Biography = x.Biography,
                                                   ProviderSkills = x.ProviderSkills
                                               .Select(a => new ProviderSkillDto
                                               {
                                                   SkillName = a.SkillName
                                               }),
                                                   Attachments = x.Attachments
                                               .Select(a => new GetFileDto
                                               {
                                                   Id = a.Id,
                                                   FileName = a.FileName,
                                                   Data = a.Data,
                                                   DocumentType = a.DocumentType,
                                               }).ToList(),
                                               })
                                              .FirstOrDefaultAsync(cancellationToken);

                if (user is null)
                    throw new NotFoundException($"User with id {userId} doesn't exist.");

                var attachments = user.Attachments.Select(a => new GetFileDto
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    Data = a.Data,
                    DocumentType = a.DocumentType,
                }).ToList();

                var providerProfile = new ProviderProfileDto
                {
                    Biography = user.Biography,
                    ProviderSkills = user.ProviderSkills,
                    IdentityCheck = attachments.Where(a => a.DocumentType == DocumentType.DBSCheck || a.DocumentType == DocumentType.PhotoOfID),
                    Certificates = attachments.Where(a => a.DocumentType == DocumentType.Certification),
                    Gallery = user.Attachments.Where(a => a.DocumentType == DocumentType.GalleryPhoto),
                    Reviews = await _reviewService.GetReviewsAsync(userId, cancellationToken)
                };

                return providerProfile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get provider profile exception:");
                throw;
            }
        }

        public async Task<CustomerProfileDto> GetCustomerProfileAsync(long userId, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users.Include(x => x.Attachments)
                                               .Include(x => x.Reviews)
                                               .Where(x => x.Id == userId && x.Attachments.Any(a => a.DocumentType == DocumentType.GalleryPhoto))
                                               .Select(x => new CustomerProfileDto
                                               {
                                                   Attachments = x.Attachments
                                               .Select(a => new GetFileDto
                                               {
                                                   Id = a.Id,
                                                   FileName = a.FileName,
                                                   Data = a.Data,
                                                   DocumentType = a.DocumentType,
                                               })
                                               .ToList(),
                                               })
                                              .FirstOrDefaultAsync(cancellationToken);

                if (user is null)
                    throw new NotFoundException($"User with id {userId} doesn't exist.");

                var attachments = user.Attachments.Select(a => new GetFileDto
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    Data = a.Data,
                    DocumentType = a.DocumentType,
                }).ToList();

                var providerProfile = new CustomerProfileDto
                {
                    Gallery = user.Attachments.Where(a => a.DocumentType == DocumentType.GalleryPhoto),
                    Reviews = await _reviewService.GetReviewsAsync(userId, cancellationToken)
                };

                return providerProfile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get customer profile exception:");
                throw;
            }
        }
    }
}

