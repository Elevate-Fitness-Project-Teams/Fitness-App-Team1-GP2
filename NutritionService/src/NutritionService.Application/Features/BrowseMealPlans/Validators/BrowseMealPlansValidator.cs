using NutritionService.Application.Common.Exceptions;
using NutritionService.Application.Features.BrowseMealPlans.Queries;

namespace NutritionService.Application.Features.BrowseMealPlans.Validators;

public static class BrowseMealPlansValidator
{
    public static void EnsureValid(BrowseMealPlansQuery q)
    {
        if (q.Page < 1)
            throw new ValidationAppException("VAL_PAGE_INVALID", "page must be >= 1.");
        if (q.PageSize < 1 || q.PageSize > 100)
            throw new ValidationAppException("VAL_PAGE_SIZE_INVALID", "pageSize must be between 1 and 100.");
    }
}
