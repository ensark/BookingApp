namespace Booking.Core.Domain.DTOs
{
    public class ProviderSearchListDto
    {
        public byte[] ProfileImage { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string City { get; set; }               
        public decimal Rank { get; set; }
    }
}
