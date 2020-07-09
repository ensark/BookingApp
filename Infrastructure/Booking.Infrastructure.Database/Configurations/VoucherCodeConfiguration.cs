using Booking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Database.Configurations
{
    public class VoucherCodeConfiguration : BaseEntityConfiguration<VoucherCode, long>
    {
        public override void Configure(EntityTypeBuilder<VoucherCode> builder)
        {
            base.Configure(builder);
            builder.ToTable("VoucherCodes");
            builder.Property(x => x.Code).HasColumnName("Code").IsRequired();
            builder.Property(x => x.IsUsed).HasColumnName("IsUsed").IsRequired();
            builder.Property(x => x.UserId).HasColumnName("UserId").IsRequired();
        }
    }
}
