using System.Collections.Generic;
using Booking.Core.Domain.Enums;
using Booking.Core.Domain.Interfaces;

namespace Booking.Core.Domain.Entities
{
    public class Provider : BaseEntity<long>, IDeleted
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public ServiceType ServiceType { get; set; }
        public ProfessionType ProfessionType { get; set; }
        public decimal PricePerSession { get; set; }
        public int? NumberOfParticipants { get; set; }
        public float? FiveSessionsDiscount { get; set; }
        public float? TenSessionsDiscount { get; set; }
        public string ProviderStrypeId { get; set; }

        public virtual Location Location { get; set; }
        public virtual ScheduleSettings ScheduleSettings { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }

        public virtual User User { get; set; }
        public long UserId { get; set; }

        public bool Deleted { get; set; }
    }
}
