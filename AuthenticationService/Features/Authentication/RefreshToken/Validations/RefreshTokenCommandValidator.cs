using FluentValidation;

namespace AuthenticationService.Features.Authentication.RefreshToken.Validations
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.RefreshTokenRequest)
                .NotNull()
                .WithErrorCode("VAL_REQUIRED_FIELD")
                .WithMessage("Refresh token request cannot be null.");

            When(x => x.RefreshTokenRequest != null, () =>
            {
                RuleFor(x => x.RefreshTokenRequest.RefreshToken)
                    .NotEmpty()
                    .WithErrorCode("VAL_REQUIRED_FIELD")
                    .WithMessage("Refresh token is required.");
            });
        }
    }
}
