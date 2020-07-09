using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Booking.Infrastructure.Database;
using Booking.Core.Services.Interfaces;
using Booking.Core.Domain.DTOs;
using Microsoft.Extensions.Logging;
using Booking.Infrastructure.Common.Interfaces;
using Booking.Core.Domain.Enums;

namespace Booking.Infrastructure.SignalR.Chat.Hubs
{
    //[Authorize]
    public class ChatHub : Hub
    {
        private readonly BookingDBContext _context;
        private readonly ILogger<ChatHub> _logger;
        private readonly IHubConnectionService _hubConnectionService;
        private readonly IChatService _chatService;
        private readonly IFirebaseMessageClient _firebaseClient;

        public ChatHub(BookingDBContext context, ILogger<ChatHub> logger, IHubConnectionService hubConnectionService, IChatService chatService, IFirebaseMessageClient firebaseClient)
        {
            _context = context;
            _logger = logger;
            _hubConnectionService = hubConnectionService;
            _chatService = chatService;
            _firebaseClient = firebaseClient;
        }

        /// <summary>
        /// When login be finish and pass token in Header replace hard coded values with Convert.ToInt64(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        /// </summary>
        /// <param name="sendTo"></param> This is (userId)
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessage(string sendTo, string message)
        {
            try
            {
                var senderId = 3;   // Convert.ToInt64(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value); 
                var receiverId = 2;

                if (receiverId > 0)
                {
                    var receiverConnections = await _context.HubConnections.Where(x => x.UserId == receiverId).ToListAsync();

                    var chatMessage = new AddChatDto
                    {
                        SenderId = senderId,
                        ReceiverId = receiverId,
                        ChatMessage = new AddChatMessageDto
                        {
                            MessageContent = message,
                            MessageSentAt = DateTime.Now,
                            IsRead = true
                        }
                    };

                    await _chatService.SaveChatMessagesAsync(chatMessage, CancellationToken.None);

                    if (receiverConnections.Any())
                    {
                        var senderConnections = await _context.HubConnections.Where(x => x.UserId == senderId).ToListAsync();

                        IEnumerable<string> allReceivers = senderConnections.Select(conn => conn.ConnectionId)
                            .Concat(receiverConnections.Select(conn => conn.ConnectionId));

                        foreach (string connectionId in allReceivers)
                        {
                            await Clients.Client(connectionId).SendAsync("ReceiveMessage", sendTo, message);
                        }

                        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == receiverId);

                        if (user.NotificationSettings.PrivateMessages)
                        {
                            var pushNotificationRequest = new PushNotificationRequestDto
                            {
                                DeviceId = user.FcmTokenDeviceId,
                                Title = "New message",
                                Body = $"{message}",
                                Data = null,
                                SenderId = senderId,
                                ReceiverId = receiverId,
                                NotificationType = NotificationType.PrivateMessages
                            };

                            await _firebaseClient.SendPushNotificationAsync(pushNotificationRequest, CancellationToken.None);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Send message service exception: ");
                throw;
            }
        }

        /// <summary>
        ///  When login be finish and pass token in Header replace hard coded values with Convert.ToInt64(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        /// </summary>
        /// <returns></returns>
        public async override Task OnConnectedAsync()
        {
            var userId = 2; //Convert.ToInt64(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value); 
            var connectionId = Context.ConnectionId;

            await _hubConnectionService.SaveHubConnectionAsync(userId, connectionId, CancellationToken.None);

            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception ex)
        {
            var connectionId = Context.ConnectionId;

            var hubConnection = _context.HubConnections.FirstOrDefault(conn => conn.ConnectionId == connectionId);

            if (hubConnection != null)
            {
                _context.HubConnections.Remove(hubConnection);
                await _context.SaveChangesAsync();
            }

            await base.OnDisconnectedAsync(ex);
        }
    }
}
