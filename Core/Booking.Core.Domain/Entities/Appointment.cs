using System;
using Booking.Core.Domain.Enums;

namespace Booking.Core.Domain.Entities
{
    public class Appointment : BaseEntity<long>
    {
        public DateTime AppointmentTime { get; set; }
        public AppointmentStatus AppointmentStatus { get; set; }
        public decimal PricePerSession { get; set; }
        public decimal? PricePerSessionDiscount { get; set; }
     
        public virtual Reservation Reservation { get; set; }
        public long ReservationId { get; set; }

        public Guid AppointmentExternalId { get; set; }
        public string PayPalApprovalLink { get; set; }
    }
}
