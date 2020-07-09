using System;

namespace Booking.Core.Domain.DTOs
{
    public class AddChatMessageDto
    {
        public string MessageContent { get; set; }
        public DateTime MessageSentAt { get; set; }
        public bool IsRead { get; set; }        
    }
}
