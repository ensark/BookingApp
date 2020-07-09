namespace Booking.Core.Domain.Entities
{
    public class VoucherCode : BaseEntity<long>
    {
        public string Code { get; set; }
        public bool IsUsed { get; set; }

        public virtual User User { get; set; }
        public long UserId { get; set; }
    }
}
