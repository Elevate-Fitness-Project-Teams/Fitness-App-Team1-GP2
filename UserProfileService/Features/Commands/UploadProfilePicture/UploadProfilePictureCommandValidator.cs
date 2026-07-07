using FluentValidation;
using System.IO;
using System.Linq;

namespace FitnessApp.UserProfileService.Features.Commands.UploadProfilePicture
{
    public class UploadProfilePictureCommandValidator : AbstractValidator<UploadProfilePictureCommand>
    {
        public UploadProfilePictureCommandValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithErrorCode("AUTH_TOKEN_INVALID");

            RuleFor(x => x.ProfilePicture)
                .NotNull()
                .WithErrorCode("VAL_REQUIRED_FIELD")
                .DependentRules(() =>
                {
                    RuleFor(x => x.ProfilePicture)
                        .Must(file =>
                        {
                            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                            return new[] { ".jpg", ".jpeg", ".png" }.Contains(extension);
                        })
                        .WithErrorCode("VAL_INVALID_FILE_TYPE")
                        .WithMessage("Only JPG and PNG files are allowed.");

                    RuleFor(x => x.ProfilePicture)
                        .Must(file => file.Length <= 5 * 1024 * 1024)
                        .WithErrorCode("VAL_FILE_TOO_LARGE")
                        .WithMessage("File size cannot exceed 5MB.");
                });
        }
    }
}
