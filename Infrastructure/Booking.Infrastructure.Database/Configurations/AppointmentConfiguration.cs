using Booking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Database.Configurations
{
    public class AppointmentConfiguration : BaseEntityConfiguration<Appointment, long>
    {
        public override void Configure(EntityTypeBuilder<Appointment> builder)
        {
            base.Configure(builder);
            builder.ToTable("Appointments");
            builder.Property(x => x.AppointmentTime).HasColumnName("AppointmentTime").IsRequired();
            builder.Property(x => x.AppointmentStatus).HasColumnName("AppointmentStatusId").IsRequired();
            builder.Property(x => x.PricePerSession).HasColumnName("PricePerSession").HasColumnType("decimal(18, 4)").IsRequired();
            builder.Property(x => x.PricePerSessionDiscount).HasColumnName("PricePerSessionDiscount").HasColumnType("decimal(18, 4)");
            builder.Property(x => x.ReservationId).HasColumnName("ReservationId").IsRequired();
            builder.Property(x => x.AppointmentExternalId).HasColumnName("AppointmentExternalId").IsRequired();
        }
    }
}
