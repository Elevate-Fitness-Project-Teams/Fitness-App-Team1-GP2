using FluentValidation;

namespace FitnessApp.UserProfileService.Features.Commands.UpdateProfile
{
    public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
    {
        public UpdateProfileCommandValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithErrorCode("VAL_REQUIRED_FIELD")
                .WithMessage("Valid user identifier is required.");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithErrorCode("VAL_REQUIRED_FIELD")
                .MaximumLength(50);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithErrorCode("VAL_REQUIRED_FIELD")
                .MaximumLength(50);


            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithErrorCode("VAL_REQUIRED_FIELD")
                .Matches(@"^(\+20|0)?1[0125]\d{8}$")
                .WithErrorCode("VAL_INVALID_PHONE");
        }
    }
}
