using Booking.Core.Domain.Entities;
using Booking.Core.Services.Interfaces;
using Booking.Infrastructure.Database;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Booking.Core.Services
{
    public class HubConnectionService: IHubConnectionService
    {
        private readonly BookingDBContext _context;
        private readonly ILogger<HubConnectionService> _logger;

        public HubConnectionService(BookingDBContext context, ILogger<HubConnectionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SaveHubConnectionAsync(long userId, string connectionId, CancellationToken cancellationToken)
        {
            try
            {
                var hubConnection = new HubConnection
                {
                    UserId = userId,
                    ConnectionId = connectionId
                };

                _context.HubConnections.Add(hubConnection);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Save hub connection service exception: ");
                throw;
            }
        }
    }
}
