using FluentValidation;
using Booking.Core.Domain.DTOs;
using Booking.Core.Services.Interfaces;

namespace Booking.Common.Validations
{
    public class CreateUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public CreateUserDtoValidator(IValidationService validationService)
        {
            RuleFor(n => n.FirstName)
                    .NotEmpty().WithMessage("FirstName is requried")
                    .MinimumLength(2)
                    .MaximumLength(250);

            RuleFor(n => n.LastName)
                    .NotEmpty().WithMessage("LastName is requried")
                    .MinimumLength(2)
                    .MaximumLength(250);

            RuleFor(n => n.Email)
                    .NotEmpty().WithMessage("Email is requried")
                    .EmailAddress().WithMessage("A valid email address is required")
                    .MustAsync(async (email, token) =>
                    {
                        return await validationService.IsEmailTaken(email, token);
                    })
                    .WithMessage("{PropertyName} {PropertyValue} is already taken.");

            RuleFor(n => n.Password)
                    .NotEmpty().WithMessage("Password is requried")
                    .MinimumLength(8)
                    .Matches("[^a-zA-Z0-9]").WithMessage("Your password must be between 8 - 15 characters and must contain at least three of the following: upper case letter, lower case letter, number, symbol!")
                    .MaximumLength(15);

            RuleFor(n => n.Phone)
                    .NotEmpty().WithMessage("Phone is requried")
                    .MustAsync(async (phone, token) =>
                    {
                        return await validationService.ValidatePhoneNumber(phone, token);
                    })
                    .WithMessage("{PropertyName} {PropertyValue} must be in E.164 format up to fifteen digits in length starting with a +.");

            RuleFor(n => n.FcmTokenDeviceId)
                    .NotEmpty().WithMessage("Firebase token/deviceId is required");

            RuleFor(n => n.UserType)
                    .IsInEnum();

            RuleFor(n => n.Address.Street)
                    .NotEmpty().OverridePropertyName("Street").WithMessage("Street is requried")
                    .MinimumLength(2)
                    .MaximumLength(250);

            RuleFor(n => n.Address.City)
                    .NotEmpty().OverridePropertyName("City").WithMessage("City is requried")
                    .MinimumLength(2)
                    .MaximumLength(250);

            RuleFor(n => n.Address.Postcode)
                    .NotEmpty().OverridePropertyName("Postcode").WithMessage("Postcode is requried")
                    .MinimumLength(2)
                    .MaximumLength(250);

            RuleFor(n => n.Address.Country)
                    .NotEmpty().OverridePropertyName("Country").WithMessage("Country is requried")
                    .MinimumLength(2)
                    .MaximumLength(250);
        }
    }
}
