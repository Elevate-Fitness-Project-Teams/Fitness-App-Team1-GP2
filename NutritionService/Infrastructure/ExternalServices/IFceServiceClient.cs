using NutritionService.Infrastructure.ExternalServices.Models;

namespace NutritionService.Infrastructure.ExternalServices;

/// <summary>
/// Synchronous REST client Nutrition Service -> FCE Service, used to read the
/// caller's currently active CalorieTarget (per Epic 6 backend notes).
/// </summary>
public interface IFceServiceClient
{
    Task<CalorieTargetResponse?> GetCalorieTargetAsync(Guid userId, CancellationToken cancellationToken = default);
}
