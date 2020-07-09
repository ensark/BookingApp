using Booking.Core.Domain.Enums;

namespace Booking.Core.Domain.Entities
{
    public class Connection : BaseEntity<long>
    {
        public virtual User Customer { get; set; }
        public long CustomerId { get; set; }

        public virtual User Provider { get; set; }
        public long ProviderId { get; set; }

        public bool CustomerSentRequest { get; set; }
        public bool ProviderSentRequest { get; set; }
        public ConnectionStatus ConnectionStatus { get; set; }
    }
}
