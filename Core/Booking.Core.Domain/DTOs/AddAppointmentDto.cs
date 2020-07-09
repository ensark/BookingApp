using System;
using Booking.Core.Domain.Enums;

namespace Booking.Core.Domain.DTOs
{
    public class AddAppointmentDto
    {
        public DateTime AppointmentTime { get; set; }
        public AppointmentStatus AppointmentStatus { get; set; }
        public decimal PricePerSession { get; set; }
        public decimal PricePerSessionDiscount { get; set; }
        public long ReservationId { get; set; }
        public Guid AppointmentExternalId { get; set; }
    }
}
