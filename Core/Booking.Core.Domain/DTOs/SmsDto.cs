namespace Booking.Core.Domain.DTOs
{
    public class SmsDto
    {
        public string ReceiverNumber { get; set; }
        public string SenderNumber { get; set; }
        public string Message { get; set; }
    }
}