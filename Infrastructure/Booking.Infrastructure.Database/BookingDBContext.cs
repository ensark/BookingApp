using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Booking.Core.Domain.Entities;
using Booking.Core.Domain.Interfaces;

namespace Booking.Infrastructure.Database
{
    public class BookingDBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<ScheduleSettings> ScheduleSettings { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<HubConnection> HubConnections { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ProviderSkill> ProviderSkills { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Invite> Invites { get; set; }
        public DbSet<VoucherCode> VoucherCodes { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<NotificationSettings> NotificationSettings { get; set; }
        public DbSet<Reminder> Reminders { get; set; }

        public BookingDBContext(DbContextOptions<BookingDBContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookingDBContext).Assembly);

            SeedData.Seed(modelBuilder);

            modelBuilder.Entity<User>()
            .HasOne(x => x.Address)
            .WithOne(a => a.User)
            .HasForeignKey<Address>(a => a.UserId);

            modelBuilder.Entity<User>()
            .HasOne(x => x.VoucherCode)
            .WithOne(a => a.User)
            .HasForeignKey<VoucherCode>(a => a.UserId);

            modelBuilder.Entity<Provider>()
            .HasOne(x => x.Location)
            .WithOne(a => a.Provider)
            .HasForeignKey<Location>(a => a.ProviderId);

            modelBuilder.Entity<Provider>()
            .HasOne(x => x.ScheduleSettings)
            .WithOne(a => a.Provider)
            .HasForeignKey<ScheduleSettings>(a => a.ProviderId);

            modelBuilder.Entity<User>()
            .HasMany(x => x.Reviews)
            .WithOne(a => a.ReviewerUser)
            .HasForeignKey(a => a.ReviewerId);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            MarkAsDeleted();

            return base.SaveChangesAsync(cancellationToken);
        }

        private void MarkAsDeleted()
        {
            ChangeTracker.DetectChanges();

            var markedAsDeletedOrModified = ChangeTracker.Entries().Where(x => x.State == EntityState.Deleted || x.State == EntityState.Modified);

            foreach (var item in markedAsDeletedOrModified)
            {
                if (item.Entity is IBaseEntity baseEntity)
                    baseEntity.ModifiedDate = DateTime.UtcNow;

                if (item.Entity is IDeleted entity && item.State == EntityState.Deleted)
                {
                    item.State = EntityState.Unchanged;

                    entity.Deleted = true;
                }
            }
        }
    }
}
