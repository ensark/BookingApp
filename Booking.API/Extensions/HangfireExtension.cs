using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Hangfire;
using Hangfire.SqlServer;
using Booking.Core.Services.Interfaces;

namespace Booking.API.Extensions
{
    public static class HangfireExtension
    {
        public static void AddHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(config => config
              .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
              .UseSimpleAssemblyNameTypeSerializer()
              .UseRecommendedSerializerSettings()
              .UseSqlServerStorage(configuration.GetConnectionString(Constants.Config.HANGFIRE_CONNECTION_STRING), new SqlServerStorageOptions
              {
                  CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                  SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                  QueuePollInterval = TimeSpan.Zero,
                  UseRecommendedIsolationLevel = true,
                  UsePageLocksOnDequeue = true,
                  DisableGlobalLocks = true
              }));
        }

        public static void StartHangfire(this IApplicationBuilder app, IConfiguration configuration)
        {
            var paymentPerSessionProcessingCron = configuration.GetValue<string>(Constants.Config.HANGFIRE_PAYMENT_PER_SESSION_PROCESSING_CRON);
            var joinedUserByInviteCron = configuration.GetValue<string>(Constants.Config.HANGFIRE_CHECK_JOINED_USER_BY_INVITES);

            RecurringJob.AddOrUpdate<IPaymentService>(paymentService => paymentService.ProcessPaymentPerSessionAsync(JobCancellationToken.Null),
            paymentPerSessionProcessingCron, timeZone: TimeZoneInfo.Local);

            RecurringJob.AddOrUpdate<IInviteService>(inviteService => inviteService.CheckAcceptedInvitesAsync(JobCancellationToken.Null),
            joinedUserByInviteCron, timeZone: TimeZoneInfo.Local);
        }
    }
}
