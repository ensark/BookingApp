namespace Booking.Core.Domain.DTOs
{
    public class ReviewDto
    {
        public ReviewerDto Reviewer { get; set; }      
        public string PostDate { get; set; }
        public string Comment { get; set; }        
    }
}
