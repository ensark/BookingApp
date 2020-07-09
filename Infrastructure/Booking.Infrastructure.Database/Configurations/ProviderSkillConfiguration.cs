using Booking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Database.Configurations
{
    public class ProviderSkillConfiguration : BaseEntityConfiguration<ProviderSkill, long>
    {
        public override void Configure(EntityTypeBuilder<ProviderSkill> builder)
        {
            base.Configure(builder);
            builder.ToTable("ProviderSkills");
            builder.Property(x => x.SkillName).HasColumnName("SkillName").IsRequired();           
            builder.Property(x => x.UserId).HasColumnName("UserId").IsRequired();
        }
    }
}
