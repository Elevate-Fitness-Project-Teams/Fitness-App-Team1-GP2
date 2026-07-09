using AuthenticationService.Features.Login;
using FluentValidation;

namespace AuthenticationService.Features.Login.Validations
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.LoginRequest)
                .NotNull()
                .WithErrorCode("VAL_REQUIRED_FIELD")
                .WithMessage("Login request cannot be null.");

            When(x => x.LoginRequest != null, () =>
            {
                RuleFor(x => x.LoginRequest.Email)
                    .NotEmpty()
                    .WithErrorCode("VAL_REQUIRED_FIELD")
                    .WithMessage("Email is required.")
                    .EmailAddress()
                    .WithErrorCode("VAL_INVALID_EMAIL")
                    .WithMessage("Email must be a valid email address.");

                RuleFor(x => x.LoginRequest.Password)
                    .NotEmpty()
                    .WithErrorCode("VAL_REQUIRED_FIELD")
                    .WithMessage("Password is required.");
            });
        }
    }
}
