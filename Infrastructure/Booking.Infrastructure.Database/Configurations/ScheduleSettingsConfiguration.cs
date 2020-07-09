using Booking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Database.Configurations
{
    public class ScheduleSettingsConfiguration : BaseEntityConfiguration<ScheduleSettings, long>
    {
        public override void Configure(EntityTypeBuilder<ScheduleSettings> builder)
        {
            base.Configure(builder);
            builder.ToTable("ScheduleSettings");
            builder.Property(x => x.StartDate).HasColumnName("StartDate");
            builder.Property(x => x.EndDate).HasColumnName("EndDate");            
            builder.Property(x => x.WorkingHoursStart).HasColumnName("WorkingHoursStart");
            builder.Property(x => x.WorkingHoursEnd).HasColumnName("WorkingHoursEnd");
            builder.Property(x => x.DurationOfSessionInMinutes).HasColumnName("DurationOfSessionInMinutes");
            builder.Property(x => x.GapBetweenSessionsInMinutes).HasColumnName("GapBetweenSessionsInMinutes");            
            builder.Property(x => x.ScheduledTimeSlots).HasColumnName("ScheduledTimeSlots");
            builder.Property(x => x.ProviderId).HasColumnName("ProviderId").IsRequired();
            builder.Property(x => x.Deleted).IsRequired().HasColumnName("Deleted");

            builder.HasQueryFilter(x => !x.Deleted);
        }
    }
}
