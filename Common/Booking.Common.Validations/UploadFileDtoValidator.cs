using FluentValidation;
using Booking.Core.Domain.DTOs;
using Booking.Core.Services.Interfaces;

namespace Booking.Common.Validations
{
    public class UploadFileDtoValidator : AbstractValidator<UploadFileDto>
    {
        public UploadFileDtoValidator(IValidationService validationService)
        {
            RuleFor(n => n.File)
                    .NotEmpty().WithMessage("File is requried");

            RuleFor(n => n.DocumentType)
                    .IsInEnum();

            RuleFor(n => n.File)
                     .MustAsync(async (file, token) =>
                     {
                         return await validationService.ValidateFileSize(file, token);
                     })
                     .WithMessage("File size of image is too large. Maximum file size permitted is 1 MB.")
                     .MustAsync(async (file, token) =>
                     {
                         return await validationService.ValidateExtensionType(file, token);
                     })
                    .WithMessage("File extension is invalid.")
                    .MustAsync(async (file, type, token) =>
                     {
                         return await validationService.ValidatePhotoExtensionType(file.File, file.DocumentType, token);
                     })
                    .WithMessage("This photo extension is not allowed.");
        }
    }
}
