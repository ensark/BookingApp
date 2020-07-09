namespace Booking.Core.Domain.Enums
{
    public enum ReservationStatus
    {
        Created = 1,
        SentRequest = 2,      
        Accepted = 3,
        Rejected = 4,
        InProgress = 5,
        Paid = 6,
        Completed = 7,
        Cancelled = 8
    }
}
