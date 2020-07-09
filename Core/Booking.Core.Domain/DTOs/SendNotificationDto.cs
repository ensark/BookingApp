namespace Booking.Core.Domain.DTOs
{
    public class SendNotificationDto
    {
        public string SendTo { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public object Data { get; set; }      
    }
}
