using FluentValidation;
using Booking.Core.Domain.DTOs;
using Booking.Core.Services.Interfaces;

namespace Booking.Common.Validations
{
    public class UpdateUserPrivacySettingsDtoValidator : AbstractValidator<UpdateUserPrivacySettingsDto>
    {
        public UpdateUserPrivacySettingsDtoValidator(IValidationService validationService)
        {
            RuleFor(n => n.FirstName)
                    .NotEmpty().WithMessage("FirstName is requried")
                    .MinimumLength(2)
                    .MaximumLength(250);

            RuleFor(n => n.LastName)
                    .NotEmpty().WithMessage("LastName is requried")
                    .MinimumLength(2)
                    .MaximumLength(250);
         
            RuleFor(n => n.Phone)
                    .NotEmpty().WithMessage("Phone is requried")
                    .MustAsync(async (phone, token) =>
                    {
                        return await validationService.ValidatePhoneNumber(phone, token);
                    })
                    .WithMessage("{PropertyName} {PropertyValue} must be in E.164 format up to fifteen digits in length starting with a +.");

            RuleFor(n => n.Street)
                   .NotEmpty().OverridePropertyName("Street").WithMessage("Street is requried")
                   .MinimumLength(2)
                   .MaximumLength(250);

            RuleFor(n => n.City)
                    .NotEmpty().OverridePropertyName("City").WithMessage("City is requried")
                    .MinimumLength(2)
                    .MaximumLength(250);

            RuleFor(n => n.Postcode)
                    .NotEmpty().OverridePropertyName("Postcode").WithMessage("Postcode is requried")
                    .MinimumLength(2)
                    .MaximumLength(250);

            RuleFor(n => n.Country)
                    .NotEmpty().OverridePropertyName("Country").WithMessage("Country is requried")
                    .MinimumLength(2)
                    .MaximumLength(250);
        }
    }
}
