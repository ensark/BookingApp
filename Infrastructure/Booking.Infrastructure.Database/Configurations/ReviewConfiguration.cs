using Booking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Database.Configurations
{
    public class ReviewConfiguration : BaseEntityConfiguration<Review, long>
    {
        public override void Configure(EntityTypeBuilder<Review> builder)
        {
            base.Configure(builder);
            builder.ToTable("Reviews");          
            builder.Property(x => x.Grade).HasColumnName("Grade").HasColumnType("decimal(18, 2)").IsRequired();
            builder.Property(x => x.PostDate).HasColumnName("PostDate").IsRequired();
            builder.Property(x => x.Comment).HasColumnName("Comment").HasMaxLength(1000).IsRequired();
            builder.Property(x => x.RatedUserId).HasColumnName("RatedUserId").IsRequired();
            builder.Property(x => x.ReviewerId).HasColumnName("ReviewerId").IsRequired();
        }
    }
}
