using MediatR;
using NutritionService.Common.Abstractions;
using NutritionService.Common.Exceptions;
using NutritionService.Features.Recommendations.GetRecommendations;
using NutritionService.Infrastructure.ExternalServices;

namespace NutritionService.Features.Recommendations.GetRecommendationsByUserId;

/// <summary>Same matching logic as 6.1, explicitly scoped to a given userId (e.g. for coach/admin views).</summary>
public sealed class GetRecommendationsByUserIdQueryHandler
    : IRequestHandler<GetRecommendationsByUserIdQuery, GetRecommendationsResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFceServiceClient _fceServiceClient;

    public GetRecommendationsByUserIdQueryHandler(IUnitOfWork unitOfWork, IFceServiceClient fceServiceClient)
    {
        _unitOfWork = unitOfWork;
        _fceServiceClient = fceServiceClient;
    }

    public async Task<GetRecommendationsResult> Handle(GetRecommendationsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var calorieTarget = await _fceServiceClient.GetCalorieTargetAsync(request.UserId, cancellationToken);

        if (calorieTarget is null || !calorieTarget.IsCalculated)
            throw new BusinessRuleException("FCE_METRICS_NOT_CALCULATED", $"User {request.UserId} does not have calculated metrics yet.");

        var query = _unitOfWork.Meals.Query()
            .Where(m => m.Calories <= calorieTarget.DailyGoalCalories);

        var totalCount = query.Count();

        var meals = query
            .OrderByDescending(m => m.Protein)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => new MealRecommendationDto
            {
                Id = m.Id,
                Name = m.Name,
                Type = m.Type,
                Calories = m.Calories,
                Protein = m.Protein,
                Carbs = m.Carbs,
                Fat = m.Fat,
                Tags = m.Tags
            })
            .ToList();

        return new GetRecommendationsResult
        {
            UserDailyGoalCalories = calorieTarget.DailyGoalCalories,
            RecommendedMeals = meals,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
