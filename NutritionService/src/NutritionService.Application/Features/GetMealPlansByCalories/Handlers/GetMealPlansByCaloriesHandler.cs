using MediatR;
using Microsoft.EntityFrameworkCore;
using NutritionService.Application.Features.GetMealPlansByCalories.Dtos;
using NutritionService.Application.Features.GetMealPlansByCalories.Queries;
using NutritionService.Application.Features.GetMealPlansByCalories.Validators;
using NutritionService.Domain.Common.Interfaces;
using NutritionService.Domain.Entities;

namespace NutritionService.Application.Features.GetMealPlansByCalories.Handlers;

public sealed class GetMealPlansByCaloriesHandler
    : IRequestHandler<GetMealPlansByCaloriesQuery, IReadOnlyList<MealPlanByCaloriesDto>>
{
    private readonly IUnitOfWork _uow;

    public GetMealPlansByCaloriesHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<MealPlanByCaloriesDto>> Handle(
        GetMealPlansByCaloriesQuery request, CancellationToken cancellationToken)
    {
        var calories = GetMealPlansByCaloriesValidator.EnsureValid(request);

        return await _uow.Repository<MealPlan>()
            .Query()
            .AsNoTracking()
            .Where(p => p.TargetCalorieRangeMin <= calories && p.TargetCalorieRangeMax >= calories)
            .OrderBy(p => p.Name)
            .Select(p => new MealPlanByCaloriesDto(p.Id, p.Name, p.TargetCalorieRangeMin, p.TargetCalorieRangeMax))
            .ToListAsync(cancellationToken);
    }
}
