using Booking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Database.Configurations
{
    public class ChatMessageConfiguration : BaseEntityConfiguration<ChatMessage, long>
    {
        public override void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            base.Configure(builder);
            builder.ToTable("ChatMessages");
            builder.Property(x => x.SenderName).HasColumnName("SenderName").IsRequired();
            builder.Property(x => x.ReceiverName).HasColumnName("ReceiverName").IsRequired();
            builder.Property(x => x.Content).HasColumnName("Content").IsRequired().HasMaxLength(5000);
            builder.Property(x => x.MessageSentAt).HasColumnName("MessageSentAt").IsRequired();            
            builder.Property(x => x.ChatId).HasColumnName("ChatId").IsRequired();
            builder.Property(x => x.IsSend).HasColumnName("IsSend");
            builder.Property(x => x.IsRead).HasColumnName("IsRead");
        }
    }
}
