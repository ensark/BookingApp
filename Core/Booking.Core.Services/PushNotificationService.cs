using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Booking.Core.Domain.DTOs;
using Booking.Core.Domain.Entities;
using Booking.Core.Services.Interfaces;
using Booking.Infrastructure.Database;
using Booking.Core.Domain.Enums;
using Booking.Common.Shared;
using Booking.Core.Domain.Queries;

namespace Booking.Core.Services
{
    public class PushNotificationService : IPushNotificationService
    {
        private readonly BookingDBContext _context;
        private readonly ILogger<ChatService> _logger;

        public PushNotificationService(BookingDBContext context, ILogger<ChatService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SavePushNotificationAsync(AddNotificationDto addNotificationDto, CancellationToken cancellationToken)
        {
            try
            {
                var notification = new Notification
                {
                    CreatedBy = addNotificationDto.SenderId.ToString(),
                    SenderId = addNotificationDto.SenderId,
                    ReceiverId = addNotificationDto.ReceiverId,
                    Title = addNotificationDto.SendNotification.Title,
                    Body = addNotificationDto.SendNotification.Body,                   
                    NotificationStatus = addNotificationDto.NotificationStatus,
                    NotificationType = addNotificationDto.NoticationType,
                    ReservationId = addNotificationDto.ReservationId,
                    ConnectionId = addNotificationDto.ConnectionId,
                    NotificationSentAt = DateTime.Now
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Save push notification service exception: ");
                throw;
            }
        }

        public async Task<PagedResult<NotificationDto>> GetPushNotificationsAsync(long userId, PagedQuery pagedQuery, CancellationToken cancellationToken)
        {
            try
            {
                var pushNotifications = await _context.Notifications.Where(x => x.ReceiverId == userId && x.NotificationStatus != NotificationStatus.Failed)
                                                   .Select(x => new NotificationDto
                                                   {
                                                       SenderName = $"{x.Sender.FirstName}{x.Sender.LastName}",
                                                       ReceiverName = $"{x.Receiver.FirstName}{x.Receiver.LastName}",
                                                       Title = x.Title,
                                                       Content = x.Body,
                                                       NotificationSentAt = x.NotificationSentAt.ToString("dd MMM yyyy HH:mm")
                                                   })
                                                  .ToListAsync(cancellationToken);
              
                var totalPushNotifications = pushNotifications.Count;

                var pagedItems = pushNotifications.Skip(pagedQuery.Skip)
                                                  .Take(pagedQuery.Take);

                var pagedResult = new PagedResult<NotificationDto>
                {
                    CurrentPage = pagedQuery.Page,
                    TotalPages = pagedQuery.CalculatePages(totalPushNotifications),
                    TotalItems = totalPushNotifications,
                    Items = pagedItems
                };

                return pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get push notification service exception: ");
                throw;
            }
        }
    }
}
