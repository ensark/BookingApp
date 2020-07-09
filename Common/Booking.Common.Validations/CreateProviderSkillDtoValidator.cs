using FluentValidation;
using Booking.Core.Domain.DTOs;

namespace Booking.Common.Validations
{
    public class CreateProviderSkillDtoValidator : AbstractValidator<AddProviderSkillDto>    
    {
        public CreateProviderSkillDtoValidator()
        {
            RuleFor(n => n.SkillName)
                    .NotEmpty().WithMessage("Skill name is requried")
                    .MinimumLength(5)
                    .MaximumLength(250);
        }
    }
}
