using System.Collections.Generic;
using Booking.Core.Domain.Enums;
using Booking.Core.Domain.Interfaces;

namespace Booking.Core.Domain.Entities
{
    public class User : BaseEntity<long>, IDeleted
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Phone { get; set; }
        public UserType UserType { get; set; }       
        public string Biography { get; set; }
        public string Role { get; set; }
        public string FcmTokenDeviceId { get; set; }

        public virtual Address Address { get; set; }
        public virtual VoucherCode VoucherCode { get; set; }
        public virtual NotificationSettings NotificationSettings { get; set; }
        public virtual Reminder Reminder { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
        public virtual ICollection<ProviderSkill> ProviderSkills { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Provider> Providers { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
               
       public bool Deleted { get; set; }
    }
}
