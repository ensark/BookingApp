using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Booking.Infrastructure.Common.Interfaces;
using Booking.Core.Domain.DTOs;
using Booking.Core.Services.Interfaces;
using Booking.Core.Domain.Enums;

namespace Booking.Infrastructure.Firebase.PushNotifications
{
    public class FirebaseMessageClient : IFirebaseMessageClient
    {
        public readonly string _pushNotificationUrl;
        public readonly string _serverKeyId;
        public readonly string _senderId;

        private readonly ILogger<FirebaseMessageClient> _logger;
        private readonly IPushNotificationService _pushNotificationService;

        public FirebaseMessageClient(IConfiguration configuration, ILogger<FirebaseMessageClient> logger, IPushNotificationService pushNotificationService)
        {
            _pushNotificationUrl = configuration[PushNotificationConfig.FIREBASE_PUSH_NOTIFICATION_URL];
            _serverKeyId = configuration[PushNotificationConfig.FIREBASE_SERVER_KEY];
            _senderId = configuration[PushNotificationConfig.FIREBASE_SENDER_ID];
            _logger = logger;
            _pushNotificationService = pushNotificationService;
        }

        public async Task SendPushNotificationAsync(PushNotificationRequestDto pushNotificationRequest, CancellationToken cancellationToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_pushNotificationUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={_serverKeyId}");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Sender", $"id={_senderId}");

                    var message = new SendNotificationDto()
                    {
                        SendTo = pushNotificationRequest.DeviceId,
                        Title = pushNotificationRequest.Title,
                        Body = pushNotificationRequest.Body,
                        Data = pushNotificationRequest.Data,
                    };

                    var json = JsonConvert.SerializeObject(message);
                    var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("/fcm/send", httpContent);

                    var result = response.StatusCode.Equals(HttpStatusCode.OK);

                    var notification = new AddNotificationDto
                    {
                        SenderId = pushNotificationRequest.SenderId,
                        ReceiverId = pushNotificationRequest.ReceiverId,
                        NotificationStatus = result ? NotificationStatus.Sent : NotificationStatus.Failed,
                        NoticationType = pushNotificationRequest.NotificationType,
                        ReservationId = pushNotificationRequest.ReservationId,
                        ConnectionId = pushNotificationRequest.ConnectionId,
                        SendNotification = new SendNotificationDto
                        {
                            Title = message.Title,
                            Body = message.Body
                        }
                    };

                    await _pushNotificationService.SavePushNotificationAsync(notification, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Send push notification service exception: ");
                throw;
            }
        }
    }
}
