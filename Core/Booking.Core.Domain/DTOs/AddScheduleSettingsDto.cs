using System;
using System.Collections.Generic;

namespace Booking.Core.Domain.DTOs
{
    public class AddScheduleSettingsDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DaysOfWeek DaysOfWeek { get; set; }
        public string WorkingHoursStart { get; set; }
        public string WorkingHoursEnd { get; set; }
        public int DurationOfSessionInMinutes { get; set; }
        public int GapBetweenSessionsInMinutes { get; set; }
        public string ScheduledTimeSlots { get; set; }
        public long ProviderId { get; set; }
        public long ScheduleSettingsId { get; set; }
        public IList<string> SelectedTimeSlots { get; set; }
    }

    public class DaysOfWeek
    {
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
    }
}
