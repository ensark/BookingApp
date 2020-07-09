using System;
using Booking.Core.Domain.Interfaces;

namespace Booking.Core.Domain.Entities
{
    public class ScheduleSettings : BaseEntity<long>, IDeleted
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
      
        public string WorkingHoursStart { get; set; }
        public string WorkingHoursEnd { get; set; }
        public int DurationOfSessionInMinutes { get; set; }
        public int GapBetweenSessionsInMinutes { get; set; }
        public string ScheduledDaysOfWeek { get; set; }
        public string ScheduledTimeSlots { get; set; }

        public virtual Provider Provider { get; set; }
        public long ProviderId { get; set; }

        public bool Deleted { get; set; }
    }
}
