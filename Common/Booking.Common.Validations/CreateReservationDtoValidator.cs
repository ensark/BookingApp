using System;
using FluentValidation;
using Booking.Core.Domain.DTOs;
using Booking.Core.Services.Interfaces;
using Booking.Common.RecurrenceProcessor.Enums;

namespace Booking.Common.Validations
{
    public class CreateReservationDtoValidator : AbstractValidator<AddReservationDto>
    {
        public CreateReservationDtoValidator(IValidationService validationService)
        {
            RuleFor(n => n.ProviderId)
                   .NotEmpty().WithMessage("Provider Id is requried");

            RuleFor(n => n.RequestDate)
                   .Cascade(CascadeMode.StopOnFirstFailure)
                   .NotEmpty().WithMessage("Request date is requried")
                   .MustAsync(async (date, token) =>
                   {
                       return await validationService.ValidateInputType(date, token);
                   })
                   .WithMessage("Input date is in not correct format.")
                   .GreaterThanOrEqualTo(DateTime.Today)
                   .WithMessage("Request date must be equal or greater than today's")
                   .When(x => x.ReccurenceType == ReccurenceType.NonReccuring || x.ReccurenceType == ReccurenceType.Weekly);

            RuleForEach(n => n.RequestDates)
                   .Cascade(CascadeMode.StopOnFirstFailure)
                   .NotNull().WithMessage("Request dates are requried")
                   .MustAsync(async (date, token) =>
                   {
                       return await validationService.ValidateInputType(date, token);
                   })
                  .WithMessage("Input date is in not correct format.")
                  .GreaterThanOrEqualTo(DateTime.Today)
                  .WithMessage("Request date must be equal or greater than today's")
                  .When(x => x.ReccurenceType == ReccurenceType.Custom);

            RuleFor(n => n.RequestTime)
                   .NotEmpty().WithMessage("Request time is requried");

            RuleFor(n => n.ReccurenceType)
                   .IsInEnum();

            RuleFor(n => n.NumberOfWeeks)
                   .NotEmpty().WithMessage("Number of weeks is requried for weekly reservation type")
                   .When(x => x.ReccurenceType == ReccurenceType.Weekly);
        }
    }
}
