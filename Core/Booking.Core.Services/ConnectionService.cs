using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Booking.Core.Domain.DTOs;
using Booking.Core.Domain.Entities;
using Booking.Infrastructure.Database;
using Booking.Core.Domain.Enums;
using Booking.Core.Services.Interfaces;
using Booking.Common.Exceptions;
using Booking.Common.Shared;
using Booking.Core.Domain.Queries;
using Booking.Infrastructure.Common.Interfaces;

namespace Booking.Core.Services
{
    public class ConnectionService : IConnectionService
    {
        private readonly BookingDBContext _context;
        private readonly ILogger<ConnectionService> _logger;
        private readonly IUserService _userService;
        private readonly IFirebaseMessageClient _firebaseClient;

        public ConnectionService(BookingDBContext context, ILogger<ConnectionService> logger, IUserService userService, IFirebaseMessageClient firebaseClient)
        {
            _context = context;
            _logger = logger;
            _userService = userService;
            _firebaseClient = firebaseClient;
        }

        public async Task<UserDto> CreateConnectionRequestAsync(long userId, long connectionUserId, CancellationToken cancellationToken)
        {
            try
            {
                var connectionRequestUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

                if (_context.Connections.ToList().Exists(x => (x.CustomerId == userId && x.ProviderId == connectionUserId) ||
                                                              (x.CustomerId == connectionUserId && x.ProviderId == userId)))
                    throw new Exception("You are already connected with this user");

                var connection = new Connection
                {
                    CreatedBy = userId.ToString(),
                    CustomerId = connectionRequestUser.UserType == UserType.Customer ? userId : connectionUserId,
                    ProviderId = connectionRequestUser.UserType == UserType.ServiceProvider ? userId : connectionUserId,
                    CustomerSentRequest = connectionRequestUser.UserType == UserType.Customer ? true : false,
                    ProviderSentRequest = connectionRequestUser.UserType == UserType.ServiceProvider ? true : false,
                    ConnectionStatus = ConnectionStatus.SentRequest
                };

                _context.Connections.Add(connection);
                await _context.SaveChangesAsync(cancellationToken);

                var connectionAcceptUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == connectionUserId);

                var pushNotificationRequest = new PushNotificationRequestDto
                {
                    DeviceId = connectionAcceptUser.FcmTokenDeviceId,
                    Title = "Connection request",
                    Body = $"{connectionRequestUser.FirstName} {connectionRequestUser.LastName} sent you a connection request.",
                    Data = null,
                    SenderId = userId,
                    ReceiverId = connectionUserId,
                    ConnectionId = connection.Id,
                    NotificationType = NotificationType.ConnectionRequest
                };

                await _firebaseClient.SendPushNotificationAsync(pushNotificationRequest, cancellationToken);

                return await _userService.GetUserByIdAsync(connectionUserId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create connection service exception: ");
                throw;
            }
        }

        public async Task ConnectionRequestAnswerAsync(long connectionId, bool isAccepted, CancellationToken cancellationToken)
        {
            try
            {
                var connection = await _context.Connections.FirstOrDefaultAsync(x => x.Id == connectionId, cancellationToken);

                if (connection is null)
                    throw new Exception($"Connection with id {connectionId} not found.");

                if (isAccepted)
                    connection.ConnectionStatus = ConnectionStatus.Accepted;

                else
                    await DeleteConnectionAsync(connection.Id, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                var connectionRequestUser = await _context.Connections.Where(x => x.Id == connectionId).FirstOrDefaultAsync(cancellationToken);

                var pushMessageBody = connectionRequestUser.CustomerSentRequest ? $"{connectionRequestUser.Provider.FirstName} {connectionRequestUser.Provider.LastName}" :
                                                                                  $"{connectionRequestUser.Customer.FirstName} {connectionRequestUser.Customer.LastName}";

                var pushNotificationRequest = new PushNotificationRequestDto
                {
                    DeviceId = connectionRequestUser.CustomerSentRequest ? connectionRequestUser.Customer.FcmTokenDeviceId : connectionRequestUser.Provider.FcmTokenDeviceId,
                    Title = isAccepted ? "Connection accepted" : "Connection rejected",
                    Body = isAccepted ? $"{pushMessageBody} accepted your connection request." : $"{pushMessageBody} rejected your connection request.",
                    Data = null,
                    SenderId = connectionRequestUser.CustomerSentRequest ? connectionRequestUser.Customer.Id : connectionRequestUser.Provider.Id,
                    ReceiverId = connectionRequestUser.CustomerSentRequest ? connectionRequestUser.Provider.Id : connectionRequestUser.Customer.Id,
                    ConnectionId = connection.Id,
                    NotificationType = NotificationType.ConnectionConfirmations
                };

                await _firebaseClient.SendPushNotificationAsync(pushNotificationRequest, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reservation request answer service exception: ");
                throw;
            }
        }

        public async Task<IEnumerable<AddConnectionDto>> GetConnectionRequestsAsync(long userId, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

                var connectionRequests = user.UserType == UserType.Customer ?
                                          await _context.Connections.Where(x => x.CustomerId == userId && x.ConnectionStatus == ConnectionStatus.SentRequest)
                                    .ToListAsync(cancellationToken) :
                                   user.UserType == UserType.ServiceProvider ?
                                          await _context.Connections.Where(x => x.ProviderId == userId && x.ConnectionStatus == ConnectionStatus.SentRequest)
                                    .ToListAsync(cancellationToken) : null;

                return connectionRequests.Select(x => new AddConnectionDto
                {
                    FirstName = user.UserType == UserType.ServiceProvider ? x.Customer.FirstName : x.Provider.FirstName,
                    LastName = user.UserType == UserType.ServiceProvider ? x.Customer.LastName : x.Provider.LastName,
                    City = user.UserType == UserType.ServiceProvider ? x.Customer.Address.City : x.Provider.Address.City,
                })
                .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get connection request service exception: ");
                throw;
            }
        }

        public async Task DeleteConnectionAsync(long connectionId, CancellationToken cancellationToken)
        {
            try
            {
                var connection = await _context.Connections.FirstOrDefaultAsync(x => x.Id == connectionId, cancellationToken);

                if (connection is null)
                    throw new NotFoundException($"Connection with id {connectionId} doesn't exist.");

                _context.Connections.Remove(connection);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete connection service exception:");
                throw;
            }
        }

        public async Task<PagedResult<UserDto>> GetConnectonsAsync(long userId, SearchQuery searchQuery, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

                var connections = user.UserType == UserType.Customer ?
                                          await _context.Connections.Where(x => x.CustomerId == userId && x.ConnectionStatus == ConnectionStatus.Accepted)
                                    .ToListAsync(cancellationToken) :
                                   user.UserType == UserType.ServiceProvider ?
                                          await _context.Connections.Where(x => x.ProviderId == userId && x.ConnectionStatus == ConnectionStatus.Accepted)
                                    .ToListAsync(cancellationToken) : null;

                var query = connections.AsQueryable();

                if (!string.IsNullOrEmpty(searchQuery.QuickSearch))
                {
                    query = query.Where(x => x.Customer.FirstName.ToLower().Contains(searchQuery.QuickSearch.ToLower()) || x.Customer.LastName.ToLower().Contains(searchQuery.QuickSearch.ToLower())
                                          || x.Provider.FirstName.ToLower().Contains(searchQuery.QuickSearch.ToLower()) || x.Provider.LastName.ToLower().Contains(searchQuery.QuickSearch.ToLower()));
                }

                var totalConnections = query.Count();

                query = query.Skip(searchQuery.Skip)
                             .Take(searchQuery.Take);

                var connectionsList = query.Select(x => new UserDto
                {
                    FirstName = user.UserType == UserType.ServiceProvider ? x.Customer.FirstName : x.Provider.FirstName,
                    LastName = user.UserType == UserType.ServiceProvider ? x.Customer.LastName : x.Provider.LastName,
                    Email = user.UserType == UserType.ServiceProvider ? x.Customer.Email : x.Provider.Email,
                    Phone = user.UserType == UserType.ServiceProvider ? x.Customer.Phone : x.Provider.Phone,
                    City = user.UserType == UserType.ServiceProvider ? x.Customer.Address.Street : x.Provider.Address.Street,
                    Street = user.UserType == UserType.ServiceProvider ? x.Customer.Address.City : x.Provider.Address.City,
                    Postcode = user.UserType == UserType.ServiceProvider ? x.Customer.Address.Postcode : x.Provider.Address.Postcode,
                    Country = user.UserType == UserType.ServiceProvider ? x.Customer.Address.Country : x.Provider.Address.Country,
                })
               .ToList();

                var pagedResult = new PagedResult<UserDto>
                {
                    CurrentPage = searchQuery.Page,
                    TotalPages = searchQuery.CalculatePages(totalConnections),
                    TotalItems = totalConnections,
                    Items = connectionsList
                };

                return pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get connections service exception:");
                throw;
            }
        }
    }
}
