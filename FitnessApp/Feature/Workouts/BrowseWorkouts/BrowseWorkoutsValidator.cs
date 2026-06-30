using FluentValidation;
using WorkoutService.Feature.Workouts.BrowseWorkouts;

namespace WorkoutService.Features.Workouts.BrowseWorkouts
{
    public class BrowseWorkoutsValidator : AbstractValidator<BrowseWorkoutsQuery>
    {
        public BrowseWorkoutsValidator()
        {
            RuleFor(x => x.page)
                .GreaterThan(0)
                .WithMessage("Page must be greater than 0.");

            RuleFor(x => x.pageSize)
                .GreaterThan(0)
                .WithMessage("PageSize must be greater than 0.")
                .LessThanOrEqualTo(100)
                .WithMessage("PageSize cannot be greater than 100.");

            RuleFor(x => x.category)
                .Must(BeValidCategory)
                .When(x => !string.IsNullOrWhiteSpace(x.category))
                .WithMessage("Invalid workout category.");

            RuleFor(x => x.difficulty)
                .Must(BeValidDifficulty)
                .When(x => !string.IsNullOrWhiteSpace(x.difficulty))
                .WithMessage("Invalid workout difficulty.");

            RuleFor(x => x.duration)
                .GreaterThan(0)
                .When(x => x.duration > 0)
                .WithMessage("Duration must be greater than 0.");

            RuleFor(x => x.search)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.search))
                .WithMessage("Search cannot exceed 100 characters.");
        }

        #region Helpers
        private static readonly string[] ValidCategories =
      [
          "FullBody",
            "Chest",
            "Arms",
            "Shoulders",
            "Back",
            "Legs",
            "Stomach",
            "Push",
            "Pull",
            "UpperBody",
            "LowerBody"
      ];

        private static readonly string[] ValidDifficulties =
        [
            "beginner",
            "intermediate",
            "advanced"
        ];

        private static bool BeValidCategory(string? category)
        {
            return ValidCategories.Contains(
                category!.Trim().ToLower()
            );
        }

        private static bool BeValidDifficulty(string? difficulty)
        {
            return ValidDifficulties.Contains(
                difficulty!.Trim().ToLower()
            );
        } 
        #endregion
    }
}