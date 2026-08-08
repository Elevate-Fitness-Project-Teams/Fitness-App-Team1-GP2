using MediatR;
using NutritionService.Application.Features.GetMealDetail.Dtos;

namespace NutritionService.Application.Features.GetMealDetail.Queries;

public sealed record GetMealDetailQuery(Guid Id) : IRequest<MealDetailDto>;
