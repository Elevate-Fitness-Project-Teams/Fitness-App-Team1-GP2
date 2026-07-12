using MediatR;
using Microsoft.EntityFrameworkCore;
using NutritionService.Application.Common.Exceptions;
using NutritionService.Application.Common.Models;
using NutritionService.Application.Features.GetMealRecommendations.Dtos;
using NutritionService.Application.Features.GetMealRecommendationsByUserId.Queries;
using NutritionService.Application.Features.GetMealRecommendationsByUserId.Validators;
using NutritionService.Domain.Common.Interfaces;
using NutritionService.Domain.Entities;

namespace NutritionService.Application.Features.GetMealRecommendationsByUserId.Handlers;

public sealed class GetMealRecommendationsByUserIdHandler
    : IRequestHandler<GetMealRecommendationsByUserIdQuery, MealRecommendationsResponse>
{
    private readonly IUnitOfWork _uow;

    public GetMealRecommendationsByUserIdHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<MealRecommendationsResponse> Handle(
        GetMealRecommendationsByUserIdQuery request, CancellationToken cancellationToken)
    {
        GetMealRecommendationsByUserIdValidator.EnsureValid(request);


        var target = await _uow.Repository<UserCalorieTarget>()
            .Query()
            .AsNoTracking()
            .SingleOrDefaultAsync(t => t.UserId == request.UserId, cancellationToken);

        if (target is null)
            throw new FceMetricsNotCalculatedException(request.UserId);

        var query = _uow.Repository<Meal>().Query().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.MealType))
            query = query.Where(m => m.Type == request.MealType);

        var maxCalories = request.MaxCalories ?? target.DailyGoalCalories;
        query = query.Where(m => m.Calories <= maxCalories);

        if (request.MinProtein is decimal minProtein)
            query = query.Where(m => m.ProteinGrams >= minProtein);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(m => m.Calories)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => new MealRecommendationDto(m.Id, m.Name, m.Type, m.Calories, m.ProteinGrams))
            .ToListAsync(cancellationToken);

        var paged = totalCount == 0
            ? PagedResult<MealRecommendationDto>.Empty(request.Page, request.PageSize)
            : PagedResult<MealRecommendationDto>.Create(items, request.Page, request.PageSize, totalCount);

        return new MealRecommendationsResponse(target.DailyGoalCalories, paged);
    }
}
