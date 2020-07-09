using System.Collections.Generic;

namespace Booking.Core.Domain.Entities
{
    public class Chat : BaseEntity<long>
    {
        public virtual User Sender { get; set; }
        public long SenderId { get; set; }

        public virtual User Receiver { get; set; }
        public long ReceiverId { get; set; }

        public virtual ICollection<ChatMessage> ChatMessages { get; set; }
    }
}
