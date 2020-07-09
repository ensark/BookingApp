using Booking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Database.Configurations
{
    public class AttachmentConfiguration : BaseEntityConfiguration<Attachment, long>
    {
        public override void Configure(EntityTypeBuilder<Attachment> builder)
        {
            base.Configure(builder);
            builder.ToTable("Attachments");
            builder.Property(x => x.FileName).HasColumnName("FileName").IsRequired();
            builder.Property(x => x.DocumentType).HasColumnName("DocumentTypeId").IsRequired();
            builder.Property(x => x.ContentType).HasColumnName("ContentType").IsRequired();
            builder.Property(x => x.Data).HasColumnName("Data").IsRequired();
            builder.Property(x => x.UserId).HasColumnName("UserId");           
        }
    }
}
