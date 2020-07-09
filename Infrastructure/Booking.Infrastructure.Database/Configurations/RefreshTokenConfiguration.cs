using Booking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Database.Configurations
{
    public class RefreshTokenConfiguration : BaseEntityConfiguration<RefreshToken, long>
    {
        public override void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            base.Configure(builder);
            builder.ToTable("RefreshTokens");
            builder.Property(x => x.Token).HasColumnName("Token").IsRequired();
            builder.Property(x => x.Role).HasColumnName("Role").IsRequired();
            builder.Property(x => x.UserId).HasColumnName("UserId").IsRequired();
        }
    }
}
