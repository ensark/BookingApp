using Booking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Database.Configurations
{
    public class UserConfiguration : BaseEntityConfiguration<User, long>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);
            builder.ToTable("Users");
            builder.Property(x => x.FirstName).HasColumnName("FirstName").IsRequired();
            builder.Property(x => x.LastName).HasColumnName("LastName").IsRequired();
            builder.Property(x => x.Email).HasColumnName("Email").IsRequired();
            builder.Property(x => x.PasswordHash).HasColumnName("PasswordHash").IsRequired();
            builder.Property(x => x.PasswordSalt).HasColumnName("PasswordSalt").IsRequired();
            builder.Property(x => x.Phone).HasColumnName("Phone").IsRequired();
            builder.Property(x => x.UserType).HasColumnName("UserTypeId");         
            builder.Property(x => x.Biography).HasColumnName("Biography").HasMaxLength(5000);
            builder.Property(x => x.Role).HasColumnName("Role").IsRequired();
            builder.Property(x => x.FcmTokenDeviceId).HasColumnName("FcmTokenDeviceId").IsRequired();
            builder.Property(x => x.Deleted).IsRequired().HasColumnName("Deleted");

            builder.HasQueryFilter(x => !x.Deleted);
        }
    }
}
