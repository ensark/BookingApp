namespace Booking.Core.Domain.DTOs
{
    public class ReminderDto
    {
        public bool Booking24HoursBefore { get; set; }
        public bool Booking1HourBefore { get; set; }
        public bool Booking15MinutesBefore { get; set; }
    }
}
