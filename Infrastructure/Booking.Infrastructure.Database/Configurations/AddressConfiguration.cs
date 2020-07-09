using Booking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Database.Configurations
{
    public class AddressConfiguration : BaseEntityConfiguration<Address, long>
    {
        public override void Configure(EntityTypeBuilder<Address> builder)
        {
            base.Configure(builder);
            builder.ToTable("Addresses");
            builder.Property(x => x.Street).HasColumnName("Street").IsRequired();
            builder.Property(x => x.City).HasColumnName("City").IsRequired();
            builder.Property(x => x.Postcode).HasColumnName("Postcode").IsRequired();
            builder.Property(x => x.Country).HasColumnName("Country").IsRequired();
            builder.Property(x => x.UserId).HasColumnName("UserId").IsRequired(); ;
            builder.Property(x => x.Deleted).IsRequired().HasColumnName("Deleted");

            builder.HasQueryFilter(x => !x.Deleted);
        }
    }
}
