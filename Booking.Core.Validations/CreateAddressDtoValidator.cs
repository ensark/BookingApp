using Booking.Core.Domain.DTOs;
using FluentValidation;

namespace Booking.Core.Validations
{
    public class CreateAddressDtoValidator : AbstractValidator<AddressDto>
    {
        public CreateAddressDtoValidator()
        {
            RuleFor(n => n.Street)
                   .NotEmpty()
                   .MinimumLength(1)
                   .MaximumLength(250);

            RuleFor(n => n.City)
                   .NotEmpty()
                   .MinimumLength(1)
                   .MaximumLength(250);

            RuleFor(n => n.Postcode)
                   .NotEmpty()
                   .MinimumLength(1)
                   .MaximumLength(250);

            RuleFor(n => n.Country)
                  .NotEmpty()
                  .MinimumLength(1)
                  .MaximumLength(250);
        }
    }
}
