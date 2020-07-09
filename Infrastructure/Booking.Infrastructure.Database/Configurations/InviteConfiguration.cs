using Booking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Database.Configurations
{
    public class InviteConfiguration : BaseEntityConfiguration<Invite, long>
    {
        public override void Configure(EntityTypeBuilder<Invite> builder)
        {
            base.Configure(builder);
            builder.ToTable("Invites");
            builder.Property(x => x.InviterId).HasColumnName("InviterId").IsRequired();
            builder.Property(x => x.FriendNumber).HasColumnName("FriendNumber").IsRequired();
            builder.Property(x => x.VoucherCodeSent).HasColumnName("VoucherCodeSent").IsRequired();
        }
    }
}
