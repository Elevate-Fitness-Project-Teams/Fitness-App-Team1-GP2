using FluentValidation;
using ProgressTrackingService.Features.Progress.WorkoutLog.Request;

namespace ProgressTrackingService.Features.Progress.WorkoutLog.Validator;

public class WorkoutLogValidator : AbstractValidator<WorkoutLogRequest>
{
    public WorkoutLogValidator()
    {
        RuleFor(x => x.WorkoutId)
            .NotEmpty()
            .WithMessage("Workout id is required");

        RuleFor(x => x.SessionId)
            .NotEmpty()
            .WithMessage("Session id is required");

        RuleFor(x => x.CompletedAt)
            .NotEmpty()
            .WithMessage("CompletedAt is required");
            
        RuleFor(x => x.Duration)
            .GreaterThan(0)
            .WithMessage("Duration must be greater than 0");

        RuleFor(x => x.CaloriesBurned)
            .GreaterThanOrEqualTo(0)
            .WithMessage("CaloriesBurned must be a non-negative value");

        RuleFor(x => x.Rating)
            .IsInEnum()
            .WithMessage("Rating must be between 1 and 5");

        RuleFor(x => x.Difficulty)
            .IsInEnum()
            .WithMessage("Difficulty must be a valid enum value");

        RuleFor(x => x.ExercisesCompleted)
            .NotEmpty()
            .WithMessage("ExercisesCompleted is required");
    }
}