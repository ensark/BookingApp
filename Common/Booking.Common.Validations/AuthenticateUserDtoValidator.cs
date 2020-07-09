using FluentValidation;
using Booking.Core.Domain.DTOs;
using Booking.Core.Services.Interfaces;

namespace Booking.Common.Validations
{
    public class AuthenticateUserDtoValidator : AbstractValidator<AuthenticateUserDto>
    {
        public AuthenticateUserDtoValidator(IValidationService validationService)
        {
            RuleFor(n => n.Email)
                    .NotEmpty().WithMessage("Email is requried");

            RuleFor(n => n.Password)
                    .NotEmpty().WithMessage("Password is requried");

            RuleFor(x => x.Email)
                    .MustAsync(async (email, token) =>
                    {
                        return await validationService.DoesUserExist(email, token);
                    })
                    .OverridePropertyName("Login")
                    .WithMessage("Email or password is incorrect")
                    .MustAsync(async (email, password, token) =>
                    {
                        return await validationService.VerifyPassword(email.Email, email.Password, token);
                    })
                    .OverridePropertyName("Login")
                    .WithMessage("Email or password is incorrect");
        }
    }
}
