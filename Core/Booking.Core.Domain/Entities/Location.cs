using Booking.Core.Domain.Interfaces;
using GeoAPI.Geometries;

namespace Booking.Core.Domain.Entities
{
    public class Location : BaseEntity<long>, IDeleted
    {
        public string Name { get; set; }
        public IPoint GeoLocation { get; set; }

        public virtual Provider Provider { get; set; }
        public long ProviderId { get; set; }

        public bool Deleted { get; set; }
    }
}
