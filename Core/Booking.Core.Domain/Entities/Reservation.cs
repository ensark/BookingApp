using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Booking.Core.Domain.Enums;

namespace Booking.Core.Domain.Entities
{
    public class Reservation : BaseEntity<long>
    {
        public ReservationStatus ReservationStatus { get; set; }
        public PaymentProvider PaymentProvider { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal? TotalPriceDiscount { get; set; }
        public float? FiveSessionsDiscount { get; set; }
        public float? TenSessionsDiscount { get; set; }
        public float? VoucherCodeDiscount { get; set; }
        public string VoucherCode { get; set; }       
        public bool PayTotal { get; set; }
        public bool PayPerSession { get; set; }

        public virtual Provider Provider { get; set; }
        public long ProviderId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual User User { get; set; }
        public long CustomerId { get; set; }

        public Guid ReservationPaymentId { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}
