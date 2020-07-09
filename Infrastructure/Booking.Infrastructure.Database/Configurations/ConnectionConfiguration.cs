using Booking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Database.Configurations
{
    public class ConnectionConfiguration : BaseEntityConfiguration<Connection, long>
    {
        public override void Configure(EntityTypeBuilder<Connection> builder)
        {
            base.Configure(builder);
            builder.ToTable("Connections");
            builder.Property(x => x.CustomerId).HasColumnName("CustomerId").IsRequired();
            builder.Property(x => x.ProviderId).HasColumnName("ProviderId").IsRequired();
            builder.Property(x => x.CustomerSentRequest).HasColumnName("CustomerSentRequest").IsRequired();
            builder.Property(x => x.ProviderSentRequest).HasColumnName("ProviderSentRequest").IsRequired();
            builder.Property(x => x.ConnectionStatus).HasColumnName("ConnectionStatusId").IsRequired();
        }
    }
}
