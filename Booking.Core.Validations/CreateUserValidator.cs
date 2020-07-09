using System;
using System.Collections.Generic;
using System.Text;
using Booking.Core.Domain.DTOs;
using FluentValidation;

namespace Booking.Core.Validations
{

    public class CreateUserValidator : AbstractValidator<RegisterUserDto>
    {
        public CreateUserValidator()
        {
            RuleFor(n => n.FirstName)
               .NotNull().WithMessage("FirstName should not be null or empty!")
               .MinimumLength(1)
               .MaximumLength(250);

            RuleFor(n => n.LastName)
               .NotNull().WithMessage("LastName should not be null or empty!")
               .MinimumLength(1)
               .MaximumLength(250);

            RuleFor(n => n.Email)
               .NotEmpty()
               .MinimumLength(1)
               .MaximumLength(250);

            RuleFor(n => n.Password)
               .NotEmpty()
               .MinimumLength(1)
               .MaximumLength(250);
        }
    }
}
