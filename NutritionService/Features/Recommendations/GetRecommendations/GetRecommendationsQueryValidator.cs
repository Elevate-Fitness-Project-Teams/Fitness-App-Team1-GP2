using FluentValidation;

namespace NutritionService.Features.Recommendations.GetRecommendations;

public sealed class GetRecommendationsQueryValidator : AbstractValidator<GetRecommendationsQuery>
{
    public GetRecommendationsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.MaxCalories).GreaterThan(0).When(x => x.MaxCalories.HasValue);
        RuleFor(x => x.MinProtein).GreaterThanOrEqualTo(0).When(x => x.MinProtein.HasValue);
    }
}
