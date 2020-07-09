namespace Booking.Core.Domain.Entities
{
    public class RefreshToken : BaseEntity<long>
    {
        public string Token { get; set; }
        public string Role { get; set; }

        public virtual User User { get; set; }
        public long UserId { get; set; }
    }
}
