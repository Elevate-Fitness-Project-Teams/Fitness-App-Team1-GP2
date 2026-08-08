using FluentValidation;

namespace AuthenticationService.Features.ChangePassword.Validations
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.Request)
                .NotNull()
                .WithErrorCode("VAL_REQUIRED_FIELD")
                .WithMessage("Change password request cannot be null.");

            When(x => x.Request != null, () =>
            {
                RuleFor(x => x.Request.CurrentPassword)
                    .NotEmpty()
                    .WithErrorCode("VAL_REQUIRED_FIELD")
                    .WithMessage("Current password is required.");

                RuleFor(x => x.Request.NewPassword)
                    .NotEmpty()
                    .WithErrorCode("VAL_REQUIRED_FIELD")
                    .WithMessage("New password is required.")
                    .MinimumLength(6)
                    .WithErrorCode("AUTH_WEAK_PASSWORD")
                    .WithMessage("New password must be at least 6 characters.")
                    .Matches("[A-Z]")
                    .WithErrorCode("AUTH_WEAK_PASSWORD")
                    .WithMessage("New password must contain at least one uppercase letter.")
                    .Matches("[0-9]")
                    .WithErrorCode("AUTH_WEAK_PASSWORD")
                    .WithMessage("New password must contain at least one number.");

                RuleFor(x => x.Request.ConfirmPassword)
                    .Equal(x => x.Request.NewPassword)
                    .WithErrorCode("AUTH_PASSWORD_MISMATCH")
                    .WithMessage("Confirm password must match the new password.");
            });
        }
    }
}
