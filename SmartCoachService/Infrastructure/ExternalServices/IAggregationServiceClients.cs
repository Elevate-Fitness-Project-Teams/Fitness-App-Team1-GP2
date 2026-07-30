using SmartCoachService.Infrastructure.ExternalServices.Models;

namespace SmartCoachService.Infrastructure.ExternalServices;

public interface IProfileServiceClient
{
    Task<ProfileSummary?> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);
}

public interface IFceServiceClient
{
    Task<FceSummary?> GetCalorieTargetAsync(Guid userId, CancellationToken cancellationToken = default);
}

public interface IWorkoutServiceClient
{
    Task<WorkoutSummary?> GetTodaysWorkoutAsync(Guid userId, CancellationToken cancellationToken = default);
}

public interface INutritionServiceClient
{
    Task<NutritionSummary?> GetTopRecommendationsAsync(Guid userId, CancellationToken cancellationToken = default);
}

public interface IProgressServiceClient
{
    Task<ProgressSummary?> GetProgressSummaryAsync(Guid userId, CancellationToken cancellationToken = default);
}
