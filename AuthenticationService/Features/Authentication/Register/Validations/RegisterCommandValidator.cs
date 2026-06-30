using AuthenticationService.Features.Authentication.Register;
using FluentValidation;

namespace AuthenticationService.Features.Authentication.Register.Validations
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.RegisterRequest)
                .NotNull()
                .WithErrorCode("VAL_REQUIRED_FIELD")
                .WithMessage("Register request cannot be null.");

            When(x => x.RegisterRequest != null, () =>
            {
                RuleFor(x => x.RegisterRequest.FirstName)
                    .NotEmpty()
                    .WithErrorCode("VAL_REQUIRED_FIELD")
                    .WithMessage("First name is required.")
                    .Length(2, 50)
                    .WithErrorCode("VAL_INVALID_LENGTH")
                    .WithMessage("First name must be between 2 and 50 characters.");

                RuleFor(x => x.RegisterRequest.LastName)
                    .NotEmpty()
                    .WithErrorCode("VAL_REQUIRED_FIELD")
                    .WithMessage("Last name is required.")
                    .Length(2, 50)
                    .WithErrorCode("VAL_INVALID_LENGTH")
                    .WithMessage("Last name must be between 2 and 50 characters.");

                RuleFor(x => x.RegisterRequest.Email)
                    .NotEmpty()
                    .WithErrorCode("VAL_REQUIRED_FIELD")
                    .WithMessage("Email is required.")
                    .EmailAddress()
                    .WithErrorCode("VAL_INVALID_EMAIL")
                    .WithMessage("Email must be a valid email address.");

                RuleFor(x => x.RegisterRequest.Password)
                    .NotEmpty()
                    .WithErrorCode("VAL_REQUIRED_FIELD")
                    .WithMessage("Password is required.")
                    .MinimumLength(6)
                    .WithErrorCode("AUTH_WEAK_PASSWORD")
                    .WithMessage("Password must be at least 6 characters.")
                    .Matches("[A-Z]")
                    .WithErrorCode("AUTH_WEAK_PASSWORD")
                    .WithMessage("Password must contain at least one uppercase letter.")
                    .Matches("[0-9]")
                    .WithErrorCode("AUTH_WEAK_PASSWORD")
                    .WithMessage("Password must contain at least one number.");

                RuleFor(x => x.RegisterRequest.PhoneNumber)
                    .NotEmpty()
                    .WithErrorCode("VAL_REQUIRED_FIELD")
                    .WithMessage("Phone number is required.")
                    .Matches(@"^(\+20|0)?1[0125]\d{8}$")
                    .WithErrorCode("VAL_INVALID_PHONE")
                    .WithMessage("Phone number must match standard Egyptian format (e.g., 01012345678).");
            });
        }
    }
}
