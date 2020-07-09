using System.Collections.Generic;

namespace Booking.Core.Domain.DTOs
{
    public class ReservationDto
    {
        public string Title { get; set; }
        public string Duration { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string TotalPrice { get; set; }
        public string TotalPriceDiscount { get; set; }
        public IEnumerable<string> ScheduledAppointments { get; set; }
    }
}
 