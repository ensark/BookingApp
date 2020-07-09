using Booking.Core.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Booking.Core.Domain.DTOs
{
    public class UploadFileDto
    {
        public DocumentType DocumentType { get; set; }
        public IFormFile File { get; set; }
    }
}
