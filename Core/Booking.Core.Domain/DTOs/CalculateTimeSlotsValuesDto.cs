namespace Booking.Core.Domain.DTOs
{
    public class CalculateTimeSlotsValuesDto
    {
        public string WorkingHoursStart { get; set; }
        public string WorkingHoursEnd { get; set; }
        public int DurationOfSessionInMinutes { get; set; }
        public int GapBetweenSessionsInMinutes { get; set; }
    }
}
