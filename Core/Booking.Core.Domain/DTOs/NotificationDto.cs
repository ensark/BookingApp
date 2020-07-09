using System;

namespace Booking.Core.Domain.DTOs
{
    public class NotificationDto
    {
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
        public string NotificationSentAt { get; set; }        
    }
}
