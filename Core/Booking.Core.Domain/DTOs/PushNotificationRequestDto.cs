using Booking.Core.Domain.Enums;

namespace Booking.Core.Domain.DTOs
{
    public class PushNotificationRequestDto
    {
        public string DeviceId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public object Data { get; set; }
        public long SenderId { get; set; }
        public long ReceiverId { get; set; }
        public long? ReservationId { get; set; }
        public long? ConnectionId { get; set; }
        public NotificationType NotificationType { get; set; }
    }
}
