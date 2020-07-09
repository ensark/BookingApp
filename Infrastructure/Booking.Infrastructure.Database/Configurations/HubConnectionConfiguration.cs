using Booking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Database.Configurations
{
    public class HubConnectionConfiguration : BaseEntityConfiguration<HubConnection, long>
    {
        public override void Configure(EntityTypeBuilder<HubConnection> builder)
        {
            base.Configure(builder);
            builder.ToTable("HubConnections");
            builder.Property(x => x.UserId).HasColumnName("UserId").IsRequired();
            builder.Property(x => x.ConnectionId).HasColumnName("ConnectionId").IsRequired().HasMaxLength(256);
        }
    }
}
