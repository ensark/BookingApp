using System;

namespace Booking.Core.Domain.DTOs
{
    public class LoggedUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }       
        public string City { get; set; }
        public string Title { get; set; }
        public byte[] ProfileImage { get; set; }
        public decimal Rank { get; set; }

        public string AccessToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; }
        public string RefreshToken { get; set; }
    }
}
