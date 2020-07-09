using System;
using System.Collections.Generic;
using Booking.Common.RecurrenceProcessor.Enums;

namespace Booking.Core.Domain.DTOs
{
    public class AddReservationDto
    {
        public long ProviderId { get; set; }        
        public DateTime RequestDate { get; set; }
        public string RequestTime { get; set; }
        public IEnumerable<DateTime> RequestDates { get; set; }
        public ReccurenceType ReccurenceType { get; set; }
        public int NumberOfWeeks { get; set; }          
        public string ReservationPaymentId { get; set; }        
    }
}
