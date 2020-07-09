namespace Booking.Core.Domain.DTOs
{
    public class NotificationSettingsDto
    {
        public bool BookingConfirmations { get; set; }
        public bool RecommendationRequestFromFriends { get; set; }
        public bool PrivateMessages { get; set; }
        public bool NewBookings { get; set; }
        public bool AutomaticBookingConfirmation { get; set; }
    }
}
