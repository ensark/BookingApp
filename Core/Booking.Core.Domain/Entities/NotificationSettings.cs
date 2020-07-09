using Booking.Core.Domain.Enums;

namespace Booking.Core.Domain.Entities
{
    public class NotificationSettings : BaseEntity<long>
    {
        public bool BookingConfirmations { get; set; }
        public bool RecommendationRequestFromFriends { get; set; }
        public bool PrivateMessages { get; set; }
        public bool NewBookings { get; set; }
        public bool AutomaticBookingConfirmation { get; set; }
        public NotificationSettingsType NotificationSettingsType { get; set; }

        public virtual User User { get; set; }
        public long? UserId { get; set; }
    }
}
