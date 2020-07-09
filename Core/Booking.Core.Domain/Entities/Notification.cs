using System;
using Booking.Core.Domain.Enums;

namespace Booking.Core.Domain.Entities
{
    public class Notification : BaseEntity<long>
    {
        public virtual User Sender { get; set; }
        public long SenderId { get; set; }

        public virtual User Receiver { get; set; }
        public long ReceiverId { get; set; }

        public string Title { get; set; }
        public string Body { get; set; }
        public NotificationStatus NotificationStatus { get; set; }
        public NotificationType NotificationType { get; set; }
        public DateTime NotificationSentAt { get; set; }
        public long? ReservationId { get; set; }
        public long? ConnectionId { get; set; }
    }
}
