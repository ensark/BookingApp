using System;
using System.Collections.Generic;

namespace Booking.Core.Domain.DTOs
{
    public class ScheduleSettingsDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string WorkingHoursStart { get; set; }
        public string WorkingHoursEnd { get; set; }
        public int DurationOfSessionInMinutes { get; set; }
        public int GapBetweenSessionsInMinutes { get; set; }

        public IEnumerable<string> ScheduledDaysOfWeek { get; set; }
        public IEnumerable<ScheduledTimeSlotsDto> ScheduledTimeSlots { get; set; }
    }
}
