using Booking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Database.Configurations
{
    public class ReminderConfiguration : BaseEntityConfiguration<Reminder, long>
    {
        public override void Configure(EntityTypeBuilder<Reminder> builder)
        {
            base.Configure(builder);
            builder.ToTable("Reminders");
            builder.Property(x => x.Booking24HoursBefore).HasColumnName("Booking24HoursBefore").IsRequired();
            builder.Property(x => x.Booking1HourBefore).HasColumnName("Booking1HourBefore").IsRequired();
            builder.Property(x => x.Booking15MinutesBefore).HasColumnName("Booking15MinutesBefore").IsRequired();      
            builder.Property(x => x.UserId).HasColumnName("UserId").IsRequired(); ;
        }      
    }
}
