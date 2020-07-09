using Microsoft.Extensions.DependencyInjection;
using Twilio.Clients;
using Booking.Core.Services;
using Booking.Core.Services.Interfaces;
using Booking.Infrastructure.Common.Interfaces;
using Booking.Infrastructure.Payments.Stripe;
using Booking.Infrastructure.Payments.PayPal;
using Booking.Infrastructure.Sms;
using Booking.Infrastructure.Firebase.PushNotifications;

namespace Booking.API.Extensions
{
    public static class IoCExtension
    {
        public static void AddIocMapping(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<IProviderService, ProviderService>();
            services.AddScoped<IReservationService, ReservationService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IStripeCardPaymentService, StripeCardPaymentService>();
            services.AddScoped<IPayPalCardPaymentService, PayPalCardPaymentService>();
            services.AddScoped<IHubConnectionService, HubConnectionService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IPushNotificationService, PushNotificationService>();
            services.AddScoped<IValidationService, ValidationService>();
            services.AddScoped<IProviderSkillService, ProviderSkillService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<ISmsService, SmsService>();
            services.AddScoped<IInviteService, InviteService>();
            services.AddScoped<IConnectionService, ConnectionService>();
            services.AddScoped<IFirebaseMessageClient, FirebaseMessageClient>();

            services.AddHttpClient<ITwilioRestClient, TwillioClient>();
        }
    }
}
