using Booking.Core.Domain.Enums;

namespace Booking.Core.Domain.DTOs
{
    public class AddProviderDto
    {
        public ProfessionType ProfessionType { get; set; }
        public ServiceType ServiceType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }      
        public decimal PricePerSession { get; set; }
        public AddLocationDto Location { get; set; }
        public AddScheduleSettingsDto ScheduleSettings { get; set; }
        public int? NumberOfParticipants { get; set; }
        public float? FiveSessionsDiscount { get; set; }
        public float? TenSessionsDiscount { get; set; }
      
    }
}
