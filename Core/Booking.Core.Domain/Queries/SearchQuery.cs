namespace Booking.Core.Domain.Queries
{
    public class SearchQuery : PagedQuery
    {
        public string QuickSearch { get; set; }
    }
}
