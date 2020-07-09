using Booking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Database.Configurations
{
    public class ChatConfiguration : BaseEntityConfiguration<Chat, long>
    {
        public override void Configure(EntityTypeBuilder<Chat> builder)
        {
            base.Configure(builder);
            builder.ToTable("Chats");
            builder.Property(x => x.SenderId).HasColumnName("SenderId").IsRequired();
            builder.Property(x => x.ReceiverId).HasColumnName("ReceiverId").IsRequired();
        }
    }
}
