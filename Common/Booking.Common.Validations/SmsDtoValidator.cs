using FluentValidation;
using Booking.Core.Domain.DTOs;
using Booking.Core.Services.Interfaces;

namespace Booking.Common.Validations
{
    public class SmsDtoValidator : AbstractValidator<SmsDto>
    {
        public SmsDtoValidator(IValidationService validationService)
        {
            RuleFor(n => n.SenderNumber)
                    .NotEmpty().WithMessage("Sender number is requried")
                    .MustAsync(async (from, token) =>
                    {
                        return await validationService.ValidatePhoneNumber(from, token);
                    })
                    .WithMessage("{PropertyName} {PropertyValue} must be in E.164 format up to fifteen digits in length starting with a +.");

            RuleFor(n => n.SenderNumber)
                    .NotEmpty().WithMessage("Receiver number is requried")
                    .MustAsync(async (from, token) =>
                    {
                        return await validationService.ValidatePhoneNumber(from, token);
                    })
                    .WithMessage("{PropertyName} {PropertyValue} must be in E.164 format up to fifteen digits in length starting with a +.");

            RuleFor(n => n.Message)
                    .NotEmpty().WithMessage("Message is requried")
                    .MinimumLength(5)
                    .MaximumLength(250);
        }
    }
}
