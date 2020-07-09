using Booking.Core.Domain.Interfaces;

namespace Booking.Core.Domain.Entities
{
    public class Address : BaseEntity<long>, IDeleted
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }

        public virtual User User { get; set; }
        public long? UserId { get; set; }
        
        public bool Deleted { get; set; }
    }
}
