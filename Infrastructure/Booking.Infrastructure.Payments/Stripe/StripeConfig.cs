namespace Booking.Infrastructure.Payments.Stripe
{
    public static class StripeConfig
    {      
        public const string STRIPE_SECRET_KEY = "Stripe:ApiKey";
        public const string STRIPE_PUBLISHABLE_KEY = "Stripe:PublishableKey";
        public const string STRIPE_WEEBHOOK_SECRET_KEY = "Stripe:WebhookSecretKey";
    }
}
