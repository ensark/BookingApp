namespace Booking.Core.Domain.DTOs
{
    public class ChatMessagesDto
    {
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string MessageContent { get; set; }
        public string MessageSentAt { get; set; }      
    }
}
