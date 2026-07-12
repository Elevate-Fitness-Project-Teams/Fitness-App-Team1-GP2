using MediatR;
using Microsoft.EntityFrameworkCore;
using NutritionService.Application.Common.Exceptions;
using NutritionService.Application.Features.GetMealDetail.Dtos;
using NutritionService.Application.Features.GetMealDetail.Queries;
using NutritionService.Domain.Common.Interfaces;
using NutritionService.Domain.Entities;
using System.Text.Json;

namespace NutritionService.Application.Features.GetMealDetail.Handlers;

public sealed class GetMealDetailHandler : IRequestHandler<GetMealDetailQuery, MealDetailDto>
{
    private readonly IUnitOfWork _uow;

    public GetMealDetailHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<MealDetailDto> Handle(GetMealDetailQuery request, CancellationToken cancellationToken)
    {
        var meal = await _uow.Repository<Meal>()
            .Query()
            .AsNoTracking()
            .SingleOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (meal is null)
            throw new MealNotFoundException(request.Id);

        return new MealDetailDto(
            meal.Id, meal.Name, meal.Type, meal.Calories,
            meal.ProteinGrams, meal.CarbsGrams, meal.FatGrams,
            JsonSerializer.Deserialize<object>(meal.IngredientsJson)!,
            JsonSerializer.Deserialize<object>(meal.InstructionsJson)!,
            JsonSerializer.Deserialize<object>(meal.VariationsJson)!,
            JsonSerializer.Deserialize<object>(meal.AllergensJson)!,
            JsonSerializer.Deserialize<object>(meal.TagsJson)!);
    }
}
