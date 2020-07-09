using Booking.Core.Domain.Enums;

namespace Booking.Core.Domain.Entities
{
    public class Attachment : BaseEntity<long>
    {
        public string FileName { get; set; }
        public DocumentType DocumentType { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
       
        public virtual User User { get; set; }
        public long? UserId { get; set; }               
    }
}
