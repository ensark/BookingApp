using System.Collections.Generic;

namespace Booking.Core.Domain.DTOs
{
    public class ReservationRequestDto
    {
        public string Title { get; set; }
        public string Duration { get; set; }              
        public IEnumerable<string> ScheduledAppointments { get; set; }
    }
}
