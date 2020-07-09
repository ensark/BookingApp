using FluentValidation;
using Booking.Core.Domain.DTOs;
using Booking.Core.Services.Interfaces;

namespace Booking.Common.Validations
{
    public class CreatePaymentProcessDtoValidator : AbstractValidator<ProcessPaymentDto>
    {
        public CreatePaymentProcessDtoValidator(IValidationService validationService)
        {
            RuleFor(n => n.ReservationId)
                  .Cascade(CascadeMode.StopOnFirstFailure)
                  .NotEmpty().WithMessage("Reservation Id is requried")
                  .MustAsync(async (reservationId, token) =>
                  {
                      return await validationService.ValidateIfReservationReadyForPay(reservationId, token);
                  })
                  .WithMessage("Reservation status before payment processing must be Accepted or InProgress.");

            RuleFor(n => n.PaymentProvider)
                   .IsInEnum();

            RuleFor(n => n.AmountPerSession)
                    .NotEmpty().WithMessage("Amount per session is requried");

            RuleFor(n => n.TotalAmount)
                    .NotEmpty().WithMessage("Total amount is requried");
                
        }
    }
}
