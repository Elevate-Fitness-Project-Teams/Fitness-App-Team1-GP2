using MediatR;
using NutritionService.Application.Common.Models;
using NutritionService.Application.Features.BrowseMealPlans.Dtos;

namespace NutritionService.Application.Features.BrowseMealPlans.Queries;

public sealed record BrowseMealPlansQuery(int Page, int PageSize) : IRequest<PagedResult<MealPlanDto>>;
