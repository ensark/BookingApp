using System.Collections.Generic;
using Booking.Core.Domain.Enums;

namespace Booking.Core.Domain.DTOs
{
    public class ProcessPaymentDto
    {
        public long ReservationId { get; set; }
        public string ProviderEmail { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public decimal AmountPerSession { get; set; }
        public decimal TotalAmount { get; set; }           
        public string Description { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public PaymentProvider PaymentProvider { get; set; }      
        public bool PayTotal { get; set; }
        public bool PayPerSession { get; set; }
        public string VoucherCode { get; set; }
        public string Token { get; set; }
                              
        public string ProviderStripeId { get; set; }
        public string PayPalApprovalLink { get; set; }
                            
        // Only for testing purpose
        public string CardHolder { get; set; }
        public string CardNumber { get; set; }
        public int ExpirationYear { get; set; }
        public int ExpirationMonth { get; set; }
        public string Cvc { get; set; }
        public string CardType { get; set; }
    }
}

