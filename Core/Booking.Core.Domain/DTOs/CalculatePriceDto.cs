namespace Booking.Core.Domain.DTOs
{
    public class CalculatePriceDto
    {
        public long ReservationId { get; set; }
        public string VoucherCode { get; set; }
        public bool PayTotal { get; set; }
    }
}
