namespace Booking.Core.Domain.DTOs
{
    public class RefreshTokenDto
    {
        public string Token { get; set; }
        public string Role { get; set; }
        public long UserId { get; set; }
    }
}
