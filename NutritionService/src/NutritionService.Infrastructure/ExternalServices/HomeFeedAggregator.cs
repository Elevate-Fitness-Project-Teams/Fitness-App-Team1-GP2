using SmartCoachService.Application.Common.Interfaces;

namespace SmartCoachService.Infrastructure.ExternalServices;

public sealed class HomeFeedAggregator : IHomeFeedAggregator
{
    private readonly IHttpClientFactory _httpFactory;

    public HomeFeedAggregator(IHttpClientFactory httpFactory) => _httpFactory = httpFactory;

    public async Task<HomeFeedPayload> AggregateAsync(Guid userId, CancellationToken ct = default)
    {
        var profileTask = FetchAsync("ProfileService", $"/api/v1/profile/{userId}", ct);
        var fceTask = FetchAsync("FceService", $"/api/v1/fce/metrics/{userId}", ct);
        var workoutTask = FetchAsync("WorkoutService", $"/api/v1/workouts/suggestions/{userId}", ct);
        var nutritionTask = FetchAsync("NutritionService", $"/api/v1/nutrition/recommendations/{userId}", ct);
        var progressTask = FetchAsync("ProgressService", $"/api/v1/progress/{userId}/snapshot", ct);

        // Fan-out: run all 5 concurrently; fail-fast if any throws.
        try
        {
            await Task.WhenAll(profileTask, fceTask, workoutTask, nutritionTask, progressTask);
        }
        catch (HttpRequestException)
        {
            throw;
        }

        return new HomeFeedPayload(
            ProfileSummary: profileTask.Result,
            FceMetrics: fceTask.Result,
            WorkoutSuggestions: workoutTask.Result,
            MealRecommendations: nutritionTask.Result,
            ProgressSnapshot: progressTask.Result);
    }

    private async Task<object> FetchAsync(string clientName, string path, CancellationToken ct)
    {
        try
        {
            var client = _httpFactory.CreateClient(clientName);
            var response = await client.GetAsync(path, ct);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(ct);
            // Return as raw object (already JSON) so the controller can pass it through.
            return System.Text.Json.JsonSerializer.Deserialize<object>(json)!;
        }
        catch (Exception ex) when (ex is not HttpRequestException)
        {
            throw new HttpRequestException($"Downstream service '{clientName}' is unavailable.", ex);
        }
    }
}
