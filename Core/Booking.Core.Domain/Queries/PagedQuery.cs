namespace Booking.Core.Domain.Queries
{
    public class PagedQuery
    {
        private const int DEFAULT_PAGE_SIZE = 5;

        private int PageSize { get; set; } = DEFAULT_PAGE_SIZE;
        public int Page { get; set; }

        public int Skip
        {
            get
            {
                EnsureValidData();
                return (Page - 1) * PageSize;
            }
        }

        public int Take { get { return PageSize; } }

        public int CalculatePages(int totalItems)
        {
            EnsureValidData();
            return (totalItems - 1) / PageSize + 1;
        }

        protected void EnsureValidData()
        {
            if (PageSize <= 0)
                PageSize = 2;

            if (PageSize > 10000)
                PageSize = 10000;

            if (Page <= 0)
                Page = 1;

            if (Page > 100000)
                Page = 100000;
        }
    }
}
