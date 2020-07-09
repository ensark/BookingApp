namespace Booking.Core.Domain.DTOs
{
    public class UpdateUserPrivacySettingsDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }       
        public string Phone { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
    }
}
