using FluentValidation;

namespace FitnessApp.UserProfileService.Features.Commands.UpdateSettings
{
    public class UpdateSettingsCommandValidator : AbstractValidator<UpdateSettingsCommand>
    {
        public UpdateSettingsCommandValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithErrorCode("VAL_REQUIRED_FIELD")
                .WithMessage("Valid user identifier is required.");

            RuleFor(x => x.Request)
                .NotNull()
                .WithErrorCode("VAL_REQUIRED_FIELD")
                .WithMessage("Request body cannot be null.");
        }
    }
}
