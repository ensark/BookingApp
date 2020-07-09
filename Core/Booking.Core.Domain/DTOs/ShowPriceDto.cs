namespace Booking.Core.Domain.DTOs
{
    public class ShowPriceDto
    {
        public decimal PricePerSession { get; set; }
        public decimal DiscountPerSessionPrice { get; set; }
        public decimal TotalPrice { get; set; }              
        public decimal DiscountTotalPrice { get; set; }
    }
}
