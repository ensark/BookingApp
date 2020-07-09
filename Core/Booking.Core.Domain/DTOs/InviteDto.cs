namespace Booking.Core.Domain.DTOs
{
    public class InviteDto
    {
        public long InviterId { get; set; }
        public string FriendNumber { get; set; }
        public bool InviteAccepted { get; set; }
    }
}
