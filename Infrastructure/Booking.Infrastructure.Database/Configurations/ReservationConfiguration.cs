using Booking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Database.Configurations
{
    public class ReservationConfiguration : BaseEntityConfiguration<Reservation, long>
    {
        public override void Configure(EntityTypeBuilder<Reservation> builder)
        {
            base.Configure(builder);
            builder.ToTable("Reservations");
            builder.Property(x => x.ReservationStatus).HasColumnName("ReservationStatusId").IsRequired();
            builder.Property(x => x.PaymentProvider).HasColumnName("PaymentProviderId");
            builder.Property(x => x.TotalPrice).HasColumnName("TotalPrice").HasColumnType("decimal(18, 4)").IsRequired();
            builder.Property(x => x.TotalPriceDiscount).HasColumnName("TotalPriceDiscount").HasColumnType("decimal(18, 4)");
            builder.Property(x => x.FiveSessionsDiscount).HasColumnName("FiveSessionsDiscount");
            builder.Property(x => x.TenSessionsDiscount).HasColumnName("TenSessionsDiscount");
            builder.Property(x => x.VoucherCodeDiscount).HasColumnName("VoucherCodeDiscount");
            builder.Property(x => x.VoucherCode).HasColumnName("VoucherCode");         
            builder.Property(x => x.PayTotal).HasColumnName("PayTotal");
            builder.Property(x => x.PayPerSession).HasColumnName("PayPerSession");
            builder.Property(x => x.ProviderId).HasColumnName("ProviderId").IsRequired();
            builder.Property(x => x.CustomerId).HasColumnName("CustomerId").IsRequired();
            builder.Property(x => x.ReservationPaymentId).HasColumnName("ReservationPaymentId").IsRequired();
        }
    }
}
