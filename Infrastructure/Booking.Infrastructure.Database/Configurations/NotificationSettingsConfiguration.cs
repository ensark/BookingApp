using Booking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Database.Configurations
{
    public class NotificationSettingsConfiguration : BaseEntityConfiguration<NotificationSettings, long>
    {
        public override void Configure(EntityTypeBuilder<NotificationSettings> builder)
        {
            base.Configure(builder);
            builder.ToTable("NotificationSettings");
            builder.Property(x => x.BookingConfirmations).HasColumnName("BookingConfirmations").IsRequired();
            builder.Property(x => x.RecommendationRequestFromFriends).HasColumnName("RecommendationRequestFromFriends").IsRequired();
            builder.Property(x => x.PrivateMessages).HasColumnName("PrivateMessages").IsRequired();
            builder.Property(x => x.NewBookings).HasColumnName("NewBookings");
            builder.Property(x => x.AutomaticBookingConfirmation).HasColumnName("AutomaticBookingConfirmation");
            builder.Property(x => x.NotificationSettingsType).HasColumnName("NotificationSettingsTypeId").IsRequired();
            builder.Property(x => x.UserId).HasColumnName("UserId");
        }
    }
}
