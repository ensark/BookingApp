using System;

namespace Booking.Core.Domain.Entities
{
    public class Review : BaseEntity<long>
    {       
        public decimal Grade { get; set; }
        public DateTime PostDate { get; set; }
        public string Comment { get; set; }

        public virtual User RatedUser { get; set; }
        public long RatedUserId { get; set; }

        public virtual User ReviewerUser { get; set; }
        public long ReviewerId { get; set; }
    }
}
