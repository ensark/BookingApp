using System.Collections.Generic;

namespace Booking.Common.Shared
{
    public class PagedResult<T> : PagedResultItems
    {
        public IEnumerable<T> Items { get; set; }
    }
}
