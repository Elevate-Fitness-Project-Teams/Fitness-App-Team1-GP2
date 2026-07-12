using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutritionService.Application.Features.BrowseMealPlans.Queries;
using NutritionService.Application.Features.GetMealDetail.Queries;
using NutritionService.Application.Features.GetMealPlansByCalories.Queries;
using NutritionService.Application.Features.GetMealRecommendations.Queries;
using NutritionService.Application.Features.GetMealRecommendationsByUserId.Queries;

namespace NutritionService.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/nutrition")]
public class NutritionController : ControllerBase
{
    private readonly IMediator _mediator;

    public NutritionController(IMediator mediator) => _mediator = mediator;

    // Story 6.1
    [HttpGet("recommendations")]
    public async Task<IActionResult> GetRecommendations(
        [FromQuery] string? mealType,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? maxCalories = null,
        [FromQuery] decimal? minProtein = null,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetMealRecommendationsQuery(mealType, page, pageSize, maxCalories, minProtein), ct);
        return Ok(result);
    }

    // Story 6.2
    [HttpGet("recommendations/{userId:guid}")]
    public async Task<IActionResult> GetRecommendationsByUserId(
        Guid userId,
        [FromQuery] string? mealType,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? maxCalories = null,
        [FromQuery] decimal? minProtein = null,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetMealRecommendationsByUserIdQuery(userId, mealType, page, pageSize, maxCalories, minProtein), ct);
        return Ok(result);
    }

    // Story 6.3
    [HttpGet("meals/{id:guid}")]
    public async Task<IActionResult> GetMealDetail(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMealDetailQuery(id), ct);
        return Ok(result);
    }

    // Story 6.4
    [HttpGet("meal-plans")]
    public async Task<IActionResult> BrowseMealPlans(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new BrowseMealPlansQuery(page, pageSize), ct);
        return Ok(result);
    }

    // Story 6.5
    [HttpGet("meal-plans/by-calories")]
    public async Task<IActionResult> GetMealPlansByCalories(
        [FromQuery] int? calories, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMealPlansByCaloriesQuery(calories), ct);
        return Ok(result);
    }
}
