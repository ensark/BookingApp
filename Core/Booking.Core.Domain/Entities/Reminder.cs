namespace Booking.Core.Domain.Entities
{
    public class Reminder : BaseEntity<long>
    {
        public bool Booking24HoursBefore { get; set; }
        public bool Booking1HourBefore { get; set; }
        public bool Booking15MinutesBefore { get; set; }

        public virtual User User { get; set; }
        public long UserId { get; set; }
    }
}
