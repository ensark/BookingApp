using Booking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Database.Configurations
{
    public class LocationConfiguration : BaseEntityConfiguration<Location, long>
    {
        public override void Configure(EntityTypeBuilder<Location> builder)
        {
            base.Configure(builder);
            builder.ToTable("Locations");
            builder.Property(x => x.Name).HasColumnName("Name").IsRequired();
            builder.Property(x => x.GeoLocation).HasColumnName("GeoLocation").IsRequired();
            builder.Property(x => x.ProviderId).HasColumnName("ProviderId").IsRequired();
            builder.Property(x => x.Deleted).IsRequired().HasColumnName("Deleted");

            builder.HasQueryFilter(x => !x.Deleted);
        }
    }
}
