namespace Booking.Core.Domain.Entities
{
    public class ProviderSkill : BaseEntity<long>
    {
        public string SkillName { get; set; }

        public virtual User User { get; set; }
        public long UserId { get; set; }
    }
}
