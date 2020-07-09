using FluentValidation;
using Booking.Core.Domain.DTOs;

namespace Booking.Common.Validations
{
    public class CreateReviewDtoValidator : AbstractValidator<AddReviewDto>
    {
        public CreateReviewDtoValidator()
        {
            RuleFor(n => n.Comment)
                        .NotEmpty().WithMessage("Comment is requried")
                        .MinimumLength(10)
                        .MaximumLength(1000);

            RuleFor(n => n.Grade)
                        .NotEmpty().WithMessage("Grade is requried");

            RuleFor(n => n.RatedUserId)
                        .NotEmpty().WithMessage("Rated user Id is requried");
        }
    }
}
