using FluentValidation;
using Booking.Core.Domain.DTOs;

namespace Booking.Common.Validations
{
    public class CreateProviderDtoValidator : AbstractValidator<AddProviderDto>
    {
        public CreateProviderDtoValidator()
        {
            RuleFor(n => n.Title)
                   .NotEmpty().WithMessage("Title is requried")
                   .MinimumLength(2)
                   .MaximumLength(250);

            RuleFor(n => n.Description)
                   .NotEmpty().WithMessage("Description is requried")
                   .MinimumLength(2)
                   .MaximumLength(250);

            RuleFor(n => n.ServiceType)
                   .IsInEnum();

            RuleFor(n => n.ProfessionType)
                    .IsInEnum();

            RuleFor(n => n.PricePerSession)
                    .NotEmpty().WithMessage("Price per session is requried");

            RuleFor(n => n.Location.Name)
                   .NotEmpty().OverridePropertyName("Location").WithMessage("Location is requried")
                   .MinimumLength(2)
                   .MaximumLength(250);

            RuleFor(n => n.Location.Longitude)
                    .NotEmpty().OverridePropertyName("Longitude").WithMessage("Longitude is requried");

            RuleFor(n => n.Location.Latitude)
                    .NotEmpty().OverridePropertyName("Latitude").WithMessage("Latitude is requried");

            RuleFor(n => n.ScheduleSettings.DaysOfWeek.Monday)
                   .NotNull().OverridePropertyName("Monday").WithMessage("Monday must be true or false");

            RuleFor(n => n.ScheduleSettings.DaysOfWeek.Tuesday)
                   .NotNull().OverridePropertyName("Tuesday").WithMessage("Tuesday must be true or false");

            RuleFor(n => n.ScheduleSettings.DaysOfWeek.Wednesday)
                   .NotNull().OverridePropertyName("Wednesday").WithMessage("Wednesday must be true or false");

            RuleFor(n => n.ScheduleSettings.DaysOfWeek.Thursday)
                   .NotNull().OverridePropertyName("Thursday").WithMessage("Thursday must be true or false");

            RuleFor(n => n.ScheduleSettings.DaysOfWeek.Friday)
                   .NotNull().OverridePropertyName("Friday").WithMessage("Friday must be true or false");

            RuleFor(n => n.ScheduleSettings.DaysOfWeek.Saturday)
                   .NotNull().OverridePropertyName("Saturday").WithMessage("Saturday must be true or false");

            RuleFor(n => n.ScheduleSettings.DaysOfWeek.Sunday)
                   .NotNull().OverridePropertyName("Sunday").WithMessage("Sunday must be true or false");

            RuleFor(n => n.ScheduleSettings.DurationOfSessionInMinutes)
                   .NotEmpty().OverridePropertyName("DurationOfSessionInMinutes").WithMessage("Duration of session in minutes is requried");

            RuleFor(n => n.ScheduleSettings.GapBetweenSessionsInMinutes)
                   .NotEmpty().OverridePropertyName("GapBetweenSessionsInMinutes").WithMessage("Gap between sessions in minutes is requried");

        }
    }
}
