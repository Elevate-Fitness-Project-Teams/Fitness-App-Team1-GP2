using NutritionService.Application.Common.Exceptions;
using NutritionService.Application.Features.GetMealRecommendations.Queries;

namespace NutritionService.Application.Features.GetMealRecommendations.Validators;

public static class GetMealRecommendationsValidator
{
    public static void EnsureValid(GetMealRecommendationsQuery q)
    {
        if (q.Page < 1)
            throw new ValidationAppException("VAL_PAGE_INVALID", "page must be >= 1.");
        if (q.PageSize < 1 || q.PageSize > 100)
            throw new ValidationAppException("VAL_PAGE_SIZE_INVALID", "pageSize must be between 1 and 100.");
    }
}
