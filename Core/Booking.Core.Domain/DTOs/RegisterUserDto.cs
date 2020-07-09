using Booking.Core.Domain.Enums;

namespace Booking.Core.Domain.DTOs
{
    public class RegisterUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public UserType UserType { get; set; }
        public string FcmTokenDeviceId { get; set; }
        public AddressDto Address { get; set; }
    }
}
