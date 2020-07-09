using System;
using System.Text;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Booking.Core.Domain.Enums;
using Booking.Core.Services.Constants;
using Booking.Core.Services.Interfaces;
using Booking.Infrastructure.Database;

namespace Booking.Core.Services
{
    public class ValidationService : IValidationService
    {
        private readonly BookingDBContext _context;
        private readonly IConfiguration _configuration;
        private readonly decimal _maxFileSize;

        public ValidationService(BookingDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _maxFileSize = Convert.ToDecimal(_configuration[Config.FILE_UPLOAD_SIZE]);
        }

        public async Task<bool> IsEmailTaken(string email, CancellationToken cancellationToken)
        {
            var emailTaken = true;

            if (await _context.Users.AnyAsync(x => x.Email == email, cancellationToken))
                emailTaken = false;

            return emailTaken;
        }

        public async Task<bool> DoesUserExist(string email, CancellationToken cancellationToken)
        {
            var userExist = true;

            var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == email, cancellationToken);

            if (user is null)
                userExist = false;

            return userExist;
        }

        public async Task<bool> VerifyPassword(string email, string password, CancellationToken cancellationToken)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == email, cancellationToken);

            if (user != null)
            {
                if (user.PasswordHash.Length != 64)
                    throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");

                if (user.PasswordSalt.Length != 128)
                    throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

                using (var hmac = new System.Security.Cryptography.HMACSHA512(user.PasswordSalt))
                {
                    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                    for (int i = 0; i < computedHash.Length; i++)
                    {
                        if (computedHash[i] != user.PasswordHash[i]) return false;
                    }
                }
            }
            return true;
        }

        public async Task<bool> ValidateFileSize(IFormFile file, CancellationToken cancellationToken)
        {
            var fileSize = true;

            if (file != null)
            {
                if (file.Length > (_maxFileSize * 1024))
                    fileSize = false;
            }

            return await Task.FromResult(fileSize);
        }

        public async Task<bool> ValidateExtensionType(IFormFile file, CancellationToken cancellationToken)
        {
            var extensionSupport = true;
            var supportedTypes = new[] { ".txt", ".doc", ".docx", ".pdf", ".xls", ".xlsx", ".jpg", ".jpeg", ".gif", ".png" };

            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();

                if (!supportedTypes.Contains(extension))
                    extensionSupport = false;
            }

            return await Task.FromResult(extensionSupport);
        }

        public async Task<bool> ValidatePhotoExtensionType(IFormFile file, DocumentType documentType, CancellationToken cancellationToken)
        {
            var photoExtensionSupport = true;
            var supportedTypes = new[] { ".jpg", ".jpeg", ".gif", ".png" };

            if (file != null && documentType == DocumentType.PhotoOfID || documentType == DocumentType.ProfileImage || documentType == DocumentType.GalleryPhoto)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();

                if (!supportedTypes.Contains(extension))
                    photoExtensionSupport = false;
            }

            return await Task.FromResult(photoExtensionSupport);
        }

        public async Task<bool> ValidatePhoneNumber(string phoneNumber, CancellationToken cancellationToken)
        {
            var phoneNumberIsValid = false;

            if (!string.IsNullOrEmpty(phoneNumber))
            {
                var correctTwillioSmsPhoneNumber = new Regex(@"^\+[1-9]\d{1,14}$");

                if (correctTwillioSmsPhoneNumber.IsMatch(phoneNumber))
                    phoneNumberIsValid = true;
            }

            return await Task.FromResult(phoneNumberIsValid);
        }

        public async Task<bool> ValidateInputType(DateTime value, CancellationToken cancellationToken)
        {
            var dateIsValid = false;

            if (value.GetType() == typeof(DateTime))
                dateIsValid = true;

            return await Task.FromResult(dateIsValid);
        }

        public async Task<bool> ValidateIfReservationReadyForPay(long reservationId, CancellationToken cancellationToken)
        {
            var readyForPaymentProcessing = false;

            if (await _context.Reservations.Where(x => x.Id == reservationId).AnyAsync(x => x.ReservationStatus == ReservationStatus.Accepted
                                                                                         || x.ReservationStatus == ReservationStatus.InProgress, cancellationToken))
                readyForPaymentProcessing = true;

            return readyForPaymentProcessing;
        }        
    }
}
