namespace Booking.Core.Domain.DTOs
{
    public class AddReviewDto
    {        
        public decimal Grade { get; set; }      
        public string Comment { get; set; }
        public long RatedUserId { get; set; }
    }
}
