using FluentValidation;
using Booking.Core.Domain.DTOs;

namespace Booking.Common.Validations
{
    public class CreateAddressDtoValidator : AbstractValidator<AddressDto>
    {
        public CreateAddressDtoValidator()
        {
            RuleFor(n => n.Street)
                    .NotEmpty().WithMessage("Street is requried")
                    .MinimumLength(2)
                    .MaximumLength(250);

            RuleFor(n => n.City)
                    .NotEmpty().WithMessage("City is requried")
                    .MinimumLength(2)
                    .MaximumLength(250);

            RuleFor(n => n.Postcode)
                    .NotEmpty().WithMessage("Postcode is requried")
                    .MinimumLength(2)
                    .MaximumLength(250);

            RuleFor(n => n.Country)
                    .NotEmpty().WithMessage("Country is requried")
                    .MinimumLength(2)
                    .MaximumLength(250);
        }
    }
}
