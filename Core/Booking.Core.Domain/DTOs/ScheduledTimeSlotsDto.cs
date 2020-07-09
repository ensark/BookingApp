using Booking.Core.Domain.Enums;

namespace Booking.Core.Domain.DTOs
{
    public class ScheduledTimeSlotsDto
    {
        public int Id { get; set; }
        public string Time { get; set; }
        public TimeSlotStatus Status { get; set; }
    }
}
