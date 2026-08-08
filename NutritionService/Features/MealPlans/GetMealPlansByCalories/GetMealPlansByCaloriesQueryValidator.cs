using FluentValidation;

namespace NutritionService.Features.MealPlans.GetMealPlansByCalories;

public sealed class GetMealPlansByCaloriesQueryValidator : AbstractValidator<GetMealPlansByCaloriesQuery>
{
    public GetMealPlansByCaloriesQueryValidator()
    {
        RuleFor(x => x.Calories)
            .NotNull().WithMessage("The 'calories' query parameter is required.")
            .GreaterThan(0).When(x => x.Calories.HasValue);
    }
}
