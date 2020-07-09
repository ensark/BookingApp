namespace Booking.API.Constants
{
    public static class Config
    {
        public const string CONNECTION_STRING = "databaseConnectionString";
        public const string HANGFIRE_CONNECTION_STRING = "hangfireConnection";
        public const string HANGFIRE_PAYMENT_PER_SESSION_PROCESSING_CRON = "Hangfire:PaymentPerSessionProcessingCron";
        public const string HANGFIRE_CHECK_JOINED_USER_BY_INVITES = "Hangfire:CheckJoinedUserByInviteCron";
    }
}


