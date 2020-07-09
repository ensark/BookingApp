using System.Collections.Generic;

namespace Booking.Core.Domain.DTOs
{
    public class ProviderProfileDto
    {
        public string Biography { get; set; }
        public IEnumerable<ProviderSkillDto> ProviderSkills { get; set; }
        public IEnumerable<GetFileDto> Attachments { get; set; }
        public IEnumerable<GetFileDto> IdentityCheck { get; set; }
        public IEnumerable<GetFileDto> Certificates { get; set; }
        public IEnumerable<GetFileDto> Gallery { get; set; }
        public IEnumerable<ReviewDto> Reviews { get; set; }
    }
}
