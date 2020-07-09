using System;

namespace Booking.Core.Domain.DTOs
{
    public class ProviderTransactionsDto
    {
        public string Customer { get; set; }
        public long Amount { get; set; }
        public DateTime Created { get; set; }
    }
}
