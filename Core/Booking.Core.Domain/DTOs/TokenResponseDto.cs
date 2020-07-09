using System;

namespace Booking.Core.Domain.DTOs
{
    public class TokenResponseDto
    {
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; }
        public string RefreshToken { get; set; }
    }
}
