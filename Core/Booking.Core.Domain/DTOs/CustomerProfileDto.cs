using System.Collections.Generic;

namespace Booking.Core.Domain.DTOs
{
    public class CustomerProfileDto
    {
        public IEnumerable<GetFileDto> Attachments { get; set; }
        public IEnumerable<GetFileDto> Gallery { get; set; }
        public IEnumerable<ReviewDto> Reviews { get; set; }
    }
}
