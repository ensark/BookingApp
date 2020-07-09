namespace Booking.Core.Domain.DTOs
{
    public class AddChatDto
    {
        public long SenderId { get; set; }
        public long ReceiverId { get; set; }      
        public AddChatMessageDto ChatMessage { get; set; }
    }
}
