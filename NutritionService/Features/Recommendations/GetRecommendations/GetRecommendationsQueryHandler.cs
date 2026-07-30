using MediatR;
using NutritionService.Common.Abstractions;
using NutritionService.Common.Exceptions;
using NutritionService.Infrastructure.ExternalServices;
using NutritionService.Infrastructure.Messaging.Events;

namespace NutritionService.Features.Recommendations.GetRecommendations;

/// <summary>
/// Given: caller already has a calculated CalorieTarget in the FCE.
/// Synchronously calls the FCE Service, matches Meals by Type + caloric range,
/// and publishes a NutritionRecommendationsGeneratedEvent for downstream analytics.
/// </summary>
public sealed class GetRecommendationsQueryHandler : IRequestHandler<GetRecommendationsQuery, GetRecommendationsResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFceServiceClient _fceServiceClient;
    private readonly ICurrentUserService _currentUser;
    private readonly IRabbitMqPublisher _publisher;

    public GetRecommendationsQueryHandler(
        IUnitOfWork unitOfWork,
        IFceServiceClient fceServiceClient,
        ICurrentUserService currentUser,
        IRabbitMqPublisher publisher)
    {
        _unitOfWork = unitOfWork;
        _fceServiceClient = fceServiceClient;
        _currentUser = currentUser;
        _publisher = publisher;
    }

    public async Task<GetRecommendationsResult> Handle(GetRecommendationsQuery request, CancellationToken cancellationToken)
    {
        var calorieTarget = await _fceServiceClient.GetCalorieTargetAsync(_currentUser.UserId, cancellationToken);

        if (calorieTarget is null || !calorieTarget.IsCalculated)
            throw new BusinessRuleException("FCE_METRICS_NOT_CALCULATED", "The caller does not have a calculated CalorieTarget yet.");

        var query = _unitOfWork.Meals.Query()
            .Where(m => m.Calories <= (request.MaxCalories ?? calorieTarget.DailyGoalCalories));

        if (request.MealType.HasValue)
            query = query.Where(m => m.Type == request.MealType.Value);

        if (request.MinProtein.HasValue)
            query = query.Where(m => m.Protein >= request.MinProtein.Value);

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

        await _publisher.PublishAsync(
            new NutritionRecommendationsGeneratedEvent(_currentUser.UserId, meals.Count, calorieTarget.DailyGoalCalories, DateTime.UtcNow),
            routingKey: "nutrition.recommendations.generated",
            cancellationToken);

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
