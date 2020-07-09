namespace Booking.Core.Domain.Entities
{
    public class Invite : BaseEntity<long>
    {
        public virtual User Inviter { get; set; }
        public long InviterId { get; set; }
        
        public string FriendNumber { get; set; }
        public bool VoucherCodeSent { get; set; }
    }
}
