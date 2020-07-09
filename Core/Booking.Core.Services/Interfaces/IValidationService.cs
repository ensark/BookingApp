using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Booking.Core.Domain.Enums;

namespace Booking.Core.Services.Interfaces
{
    public interface IValidationService
    {
        Task<bool> IsEmailTaken(string email, CancellationToken cancellationToken);

        Task<bool> DoesUserExist(string email, CancellationToken cancellationToken);

        Task<bool> VerifyPassword(string email, string password, CancellationToken cancellationToken);

        Task<bool> ValidateFileSize(IFormFile filee, CancellationToken cancellationToken);

        Task<bool> ValidateExtensionType(IFormFile file, CancellationToken cancellationToken);

        Task<bool> ValidatePhotoExtensionType(IFormFile file, DocumentType documentType, CancellationToken cancellationToken);

        Task<bool> ValidatePhoneNumber(string phoneNumber, CancellationToken cancellationToken);

        Task<bool> ValidateInputType(DateTime value, CancellationToken cancellationToken);

        Task<bool> ValidateIfReservationReadyForPay(long reservationId, CancellationToken cancellationToken);
   }
}
