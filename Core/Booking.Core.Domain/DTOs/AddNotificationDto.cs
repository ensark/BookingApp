using Booking.Core.Domain.Enums;

namespace Booking.Core.Domain.DTOs
{
    public class AddNotificationDto
    {
        public long SenderId { get; set; }
        public long ReceiverId { get; set; }
        public long? ReservationId { get; set; }
        public long? ConnectionId { get; set; }      
        public NotificationStatus NotificationStatus { get; set; }
        public NotificationType NoticationType { get; set; }

        public SendNotificationDto SendNotification { get; set; }            
    }
}
