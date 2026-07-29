using NutritionService.Application.Common.Exceptions;
using NutritionService.Application.Features.GetMealPlansByCalories.Queries;

namespace NutritionService.Application.Features.GetMealPlansByCalories.Validators;

public static class GetMealPlansByCaloriesValidator
{
    // ASSUMPTION (flagged earlier, original AC left this branch's "Then" undefined):
    // missing `calories` query param -> 400 VAL_CALORIES_REQUIRED.
    // Swap for "return the full unfiltered list" if the product owner prefers that instead.
    public static int EnsureValid(GetMealPlansByCaloriesQuery q)
    {
        if (q.Calories is not int calories)
            throw new ValidationAppException("VAL_CALORIES_REQUIRED", "Query parameter 'calories' is required.");

        return calories;
    }
}
