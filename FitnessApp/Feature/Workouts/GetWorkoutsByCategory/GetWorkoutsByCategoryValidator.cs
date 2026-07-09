using FluentValidation;

namespace WorkoutService.Feature.Workouts.GetWorkoutsByCategory;

public sealed class GetWorkoutsByCategoryValidator : AbstractValidator<GetWorkoutsByCategoryQuery>
{
    public GetWorkoutsByCategoryValidator()
    {
        RuleFor(x => x.categoryName)
            .NotEmpty()
            .WithMessage("Category name is required.");
    }
}