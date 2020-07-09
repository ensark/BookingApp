using Booking.Core.Domain.DTOs;
using FluentValidation;

namespace Booking.Core.Validations
{
    //public class CreateUserDtoValidator : AbstractValidator<RegisterUserDto>
    //{
    //    public CreateUserDtoValidator()
    //    {
    //        RuleFor(n => n.FirstName)
    //              .NotNull().WithMessage("FirstName should not be null or empty!")
    //              .MinimumLength(1)
    //              .MaximumLength(250);

    //        RuleFor(n => n.LastName)
    //              .NotNull().WithMessage("LastName should not be null or empty!")
    //              .MinimumLength(1)
    //              .MaximumLength(250);

    //        RuleFor(n => n.Email)
    //              .NotEmpty()
    //              .MinimumLength(1)
    //              .MaximumLength(250);

    //        RuleFor(n => n.Password)
    //              .NotEmpty()
    //              .MinimumLength(1)
    //              .MaximumLength(250);

    //        RuleFor(n => n.Phone)
    //              .NotEmpty()
    //              .MinimumLength(1)
    //              .MaximumLength(250);

    //        RuleFor(n => n.UserType)
    //            .IsInEnum();
    //    }
    //}
}
