using Booking.Core.Domain.Enums;

namespace Booking.Core.Domain.DTOs
{
    public class GetFileDto
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public DocumentType DocumentType { get; set; }
    }
}
