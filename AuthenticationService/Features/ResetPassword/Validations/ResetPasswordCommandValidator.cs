using FluentValidation;

namespace AuthenticationService.Features.ResetPassword.Validations
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.ResetPasswordRequest)
                .NotNull()
                .WithErrorCode("VAL_REQUIRED_FIELD")
                .WithMessage("Reset password request cannot be null.");

            When(x => x.ResetPasswordRequest != null, () =>
            {
                RuleFor(x => x.ResetPasswordRequest.ResetToken)
                    .NotEmpty()
                    .WithErrorCode("VAL_REQUIRED_FIELD")
                    .WithMessage("Reset token is required.");

                RuleFor(x => x.ResetPasswordRequest.NewPassword)
                    .NotEmpty()
                    .WithErrorCode("VAL_REQUIRED_FIELD")
                    .WithMessage("New password is required.")
                    .MinimumLength(6)
                    .WithErrorCode("AUTH_WEAK_PASSWORD")
                    .WithMessage("Password must be at least 6 characters.")
                    .Matches("[A-Z]")
                    .WithErrorCode("AUTH_WEAK_PASSWORD")
                    .WithMessage("Password must contain at least one uppercase letter.")
                    .Matches("[0-9]")
                    .WithErrorCode("AUTH_WEAK_PASSWORD")
                    .WithMessage("Password must contain at least one number.");

                RuleFor(x => x.ResetPasswordRequest.ConfirmPassword)
                    .NotEmpty()
                    .WithErrorCode("VAL_REQUIRED_FIELD")
                    .WithMessage("Confirm password is required.")
                    .Equal(x => x.ResetPasswordRequest.NewPassword)
                    .WithErrorCode("AUTH_PASSWORD_MISMATCH")
                    .WithMessage("Passwords do not match.");
            });
        }
    }
}
