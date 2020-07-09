using Booking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Database.Configurations
{
    public class NotificationConfiguration : BaseEntityConfiguration<Notification, long>
    {
        public override void Configure(EntityTypeBuilder<Notification> builder)
        {
            base.Configure(builder);
            builder.ToTable("Notifications");
            builder.Property(x => x.SenderId).HasColumnName("SenderId").IsRequired();
            builder.Property(x => x.ReceiverId).HasColumnName("ReceiverId");            
            builder.Property(x => x.Title).HasColumnName("Title").IsRequired().HasMaxLength(5000);
            builder.Property(x => x.Body).HasColumnName("Content").IsRequired();
            builder.Property(x => x.NotificationStatus).HasColumnName("NotificationStatusId").IsRequired();
            builder.Property(x => x.NotificationType).HasColumnName("NotificationTypeId").IsRequired();
            builder.Property(x => x.NotificationSentAt).HasColumnName("NotificationSentAt").IsRequired();
            builder.Property(x => x.ReservationId).HasColumnName("ReservationId");
            builder.Property(x => x.ConnectionId).HasColumnName("ConnectionId");           
        }
    }
}
