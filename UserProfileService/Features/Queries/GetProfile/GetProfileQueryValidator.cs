using FluentValidation;

namespace FitnessApp.UserProfileService.Features.Queries.GetProfile
{
    public class GetProfileQueryValidator : AbstractValidator<GetProfileQuery>
    {
        public GetProfileQueryValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithErrorCode("VAL_REQUIRED_FIELD")
                .WithMessage("Valid user identifier is required.");
        }
    }
}
