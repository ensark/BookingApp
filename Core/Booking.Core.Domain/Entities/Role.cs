namespace Booking.Core.Domain.Entities
{
    public static class Role
    {
        public const string Admin = "Adminstrator";
        public const string Customer = "Customer";
        public const string ServiceProvider = "Service Provider";
        public const string CustomerAndServiceProvider = Customer + "," + ServiceProvider;
    }
}
