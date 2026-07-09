using FluentValidation;
using WorkoutService.Feature.Workouts.StartWorkoutSessions;

namespace WorkoutService.Feature.Workouts.StartWorkoutSession;

public sealed class StartWorkoutSessionValidator : AbstractValidator<StartWorkoutSessionCommand>
{
    private static readonly string[] ValidDifficulties =
    [
        "beginner",
        "intermediate",
        "advanced"
    ];

    public StartWorkoutSessionValidator()
    {
        RuleFor(x => x.workoutId)
            .GreaterThan(0)
            .WithMessage("Workout id must be greater than 0.");

        RuleFor(x => x.userId)
            .GreaterThan(0)
            .WithMessage("User id must be greater than 0.");

        RuleFor(x => x.difficulty)
            .NotEmpty()
            .WithMessage("Difficulty is required.")
            .Must(BeValidDifficulty)
            .WithMessage("Invalid workout difficulty.");

        RuleFor(x => x.plannedDuration)
            .GreaterThan(0)
            .WithMessage("Planned duration must be greater than 0.");
    }

    private static bool BeValidDifficulty(string difficulty)
    {
        return ValidDifficulties.Contains(difficulty.Trim().ToLower());
    }
}