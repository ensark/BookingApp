using System;

namespace Booking.Core.Domain.Entities
{
    public class ChatMessage : BaseEntity<long>
    {
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }

        public string Content { get; set; }
        public DateTime MessageSentAt { get; set; }
        public bool IsSend { get; set; }
        public bool IsRead { get; set; }

        public virtual Chat Chat { get; set; }
        public long ChatId { get; set; }                           
    }
}
