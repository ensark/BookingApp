namespace Booking.Core.Domain.Entities
{
    public class HubConnection : BaseEntity<long>
    {               
        public string ConnectionId { get; set; }

        public virtual User User { get; set; }
        public long UserId { get; set; }
    }
}
