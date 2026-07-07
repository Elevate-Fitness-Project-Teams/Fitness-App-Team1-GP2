using FluentValidation;

namespace FitnessApp.UserProfileService.Features.Queries.GetProfile
{
    public class GetProfileQueryValidator : AbstractValidator<GetProfileQuery>
    {
        public GetProfileQueryValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithErrorCode("AUTH_TOKEN_INVALID")
                .WithMessage("Invalid or missing user authentication token.");
        }
    }
}
