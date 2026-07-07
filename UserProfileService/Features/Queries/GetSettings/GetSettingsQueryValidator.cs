using FluentValidation;

namespace FitnessApp.UserProfileService.Features.Queries.GetSettings
{
    public class GetSettingsQueryValidator : AbstractValidator<GetSettingsQuery>
    {
        public GetSettingsQueryValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithErrorCode("VAL_REQUIRED_FIELD")
                .WithMessage("Valid user identifier is required.");
        }
    }
}
