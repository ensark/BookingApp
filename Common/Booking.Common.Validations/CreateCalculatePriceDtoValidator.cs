using FluentValidation;
using Booking.Core.Domain.DTOs;

namespace Booking.Common.Validations
{
    public class CreateCalculatePriceDtoValidator : AbstractValidator<CalculatePriceDto>
    {
        public CreateCalculatePriceDtoValidator()
        {
            RuleFor(n => n.ReservationId)
                  .NotEmpty().WithMessage("Reservation Id is requried");     
        }
    }
}
