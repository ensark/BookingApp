using Booking.Core.Domain.Enums;
using Newtonsoft.Json;

namespace Booking.Core.Domain.DTOs
{
    public class ProviderDto
    {
        [JsonIgnore]
        public string Email { get; set; }
        [JsonIgnore]
        public string ProviderStrypeId { get; set; }

        public string ProviderName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ServiceType ServiceType { get; set; }
        public ProfessionType ProfessionType { get; set; }
        public decimal PricePerSession { get; set; }
        public int? NumberOfParticipants { get; set; }
        public float? FiveSessionsDiscount { get; set; }
        public float? TenSessionsDiscount { get; set; }
        public string LocationName { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public ScheduleSettingsDto ScheduleSettings { get; set; }
    }
}
