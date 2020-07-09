using Booking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Database.Configurations
{
    public class ProviderConfigurationonfiguration : BaseEntityConfiguration<Provider, long>
    {
        public override void Configure(EntityTypeBuilder<Provider> builder)
        {
            base.Configure(builder);
            builder.ToTable("Providers");
            builder.Property(x => x.Title).HasColumnName("Title").IsRequired();
            builder.Property(x => x.Description).HasColumnName("Description").IsRequired();
            builder.Property(x => x.ServiceType).HasColumnName("ServiceTypeId").IsRequired();
            builder.Property(x => x.ProfessionType).HasColumnName("ProfessionTypeId").IsRequired();            
            builder.Property(x => x.PricePerSession).HasColumnName("Price").HasColumnType("decimal(18, 4)").IsRequired();
            builder.Property(x => x.NumberOfParticipants).HasColumnName("NumberOfParticipants");
            builder.Property(x => x.FiveSessionsDiscount).HasColumnName("FiveSessionsDiscount");
            builder.Property(x => x.TenSessionsDiscount).HasColumnName("TenSessionsDiscount");
            builder.Property(x => x.UserId).HasColumnName("UserId").IsRequired();
            builder.Property(x => x.Deleted).IsRequired().HasColumnName("Deleted");

            builder.HasQueryFilter(x => !x.Deleted);
        }
    }
}
