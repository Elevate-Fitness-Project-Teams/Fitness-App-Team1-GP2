using MediatR;
using NutritionService.Common.Abstractions;
using NutritionService.Common.Exceptions;

namespace NutritionService.Features.Meals.GetMealDetail;

public sealed class GetMealDetailQueryHandler : IRequestHandler<GetMealDetailQuery, MealDetailDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMealDetailQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<MealDetailDto> Handle(GetMealDetailQuery request, CancellationToken cancellationToken)
    {
        var meal = await _unitOfWork.Meals.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("RES_MEAL_NOT_FOUND", $"Meal {request.Id} was not found.");

        return new MealDetailDto
        {
            Id = meal.Id,
            Name = meal.Name,
            Type = meal.Type,
            Calories = meal.Calories,
            Protein = meal.Protein,
            Carbs = meal.Carbs,
            Fat = meal.Fat,
            Ingredients = meal.Ingredients,
            Instructions = meal.Instructions,
            Variations = meal.Variations,
            Allergens = meal.Allergens,
            Tags = meal.Tags
        };
    }
}
