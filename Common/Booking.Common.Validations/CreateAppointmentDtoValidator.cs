using System;
using FluentValidation;
using Booking.Core.Domain.DTOs;
using Booking.Core.Services.Interfaces;

namespace Booking.Common.Validations
{
    public class CreateAppointmentDtoValidator : AbstractValidator<AddAppointmentDto>
    {
        public CreateAppointmentDtoValidator(IValidationService validationService)
        {            
            RuleFor(n => n.AppointmentTime)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotEmpty().WithMessage("Appointment datetime is requried")
                    .MustAsync(async (date, token) =>
                    {
                       return await validationService.ValidateInputType(date, token);
                     })
                    .WithMessage("Input date is in not correct format.")
                    .GreaterThanOrEqualTo(DateTime.Today)
                    .WithMessage("Request date must be equal or greater than today's");

            RuleFor(n => n.AppointmentStatus)
                   .IsInEnum();

            RuleFor(n => n.PricePerSession)
                   .NotEmpty().WithMessage("Price per session is requried");

            RuleFor(n => n.ReservationId)
                    .NotEmpty().WithMessage("Reservation Id is requried");

            RuleFor(n => n.AppointmentExternalId)
                    .NotEmpty().WithMessage("Appointment external Id is requried");
        }
    }
}
