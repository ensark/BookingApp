using System;
using System.Net;

namespace Booking.Core.Domain.DTOs
{
    public class PaymentResponseDto
    {
        public HttpStatusCode Status { get; set; }
        public string ProviderStrypeId { get; set; }
        public string PayPalApprovalLink { get; set; }
        public DateTime Created { get; set; }        
    }
}
