using SmartCoachService.Common.Exceptions;
using SmartCoachService.Infrastructure.ExternalServices.Models;
using System.Net;
using System.Net.Http.Json;

namespace SmartCoachService.Infrastructure.ExternalServices;

/// <summary>
/// Each downstream client follows the same shape: named HttpClient, short timeout,
/// 404 -> null, any other failure -> ServiceUnavailableException so the Home Feed
/// handler can turn a single failing dependency into SRV_SERVICE_UNAVAILABLE (503).
/// </summary>
internal static class ClientHelpers
{
    public static async Task<T?> GetOrNullAsync<T>(HttpClient client, string path, ILogger logger, CancellationToken cancellationToken)
    {
        try
        {
            var response = await client.GetAsync(path, cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound) return default;
            if (!response.IsSuccessStatusCode) throw new ServiceUnavailableException($"{client.BaseAddress} returned {(int)response.StatusCode}.");
            return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Downstream call failed: {Path}", path);
            throw new ServiceUnavailableException("A downstream aggregation call failed.");
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogError(ex, "Downstream call timed out: {Path}", path);
            throw new ServiceUnavailableException("A downstream aggregation call timed out.");
        }
    }
}

public sealed class ProfileServiceClient : IProfileServiceClient
{
    private readonly HttpClient _client;
    private readonly ILogger<ProfileServiceClient> _logger;
    public ProfileServiceClient(HttpClient client, ILogger<ProfileServiceClient> logger) { _client = client; _logger = logger; }
    public Task<ProfileSummary?> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default) =>
        ClientHelpers.GetOrNullAsync<ProfileSummary>(_client, $"/api/v1/profile/{userId}/summary", _logger, cancellationToken);
}

public sealed class FceServiceClient : IFceServiceClient
{
    private readonly HttpClient _client;
    private readonly ILogger<FceServiceClient> _logger;
    public FceServiceClient(HttpClient client, ILogger<FceServiceClient> logger) { _client = client; _logger = logger; }
    public Task<FceSummary?> GetCalorieTargetAsync(Guid userId, CancellationToken cancellationToken = default) =>
        ClientHelpers.GetOrNullAsync<FceSummary>(_client, $"/api/v1/fce/calorie-target/{userId}", _logger, cancellationToken);
}

public sealed class WorkoutServiceClient : IWorkoutServiceClient
{
    private readonly HttpClient _client;
    private readonly ILogger<WorkoutServiceClient> _logger;
    public WorkoutServiceClient(HttpClient client, ILogger<WorkoutServiceClient> logger) { _client = client; _logger = logger; }
    public Task<WorkoutSummary?> GetTodaysWorkoutAsync(Guid userId, CancellationToken cancellationToken = default) =>
        ClientHelpers.GetOrNullAsync<WorkoutSummary>(_client, $"/api/v1/workout/{userId}/today-summary", _logger, cancellationToken);
}

public sealed class NutritionServiceClient : INutritionServiceClient
{
    private readonly HttpClient _client;
    private readonly ILogger<NutritionServiceClient> _logger;
    public NutritionServiceClient(HttpClient client, ILogger<NutritionServiceClient> logger) { _client = client; _logger = logger; }
    public Task<NutritionSummary?> GetTopRecommendationsAsync(Guid userId, CancellationToken cancellationToken = default) =>
        ClientHelpers.GetOrNullAsync<NutritionSummary>(_client, $"/api/v1/nutrition/{userId}/top-summary", _logger, cancellationToken);
}

public sealed class ProgressServiceClient : IProgressServiceClient
{
    private readonly HttpClient _client;
    private readonly ILogger<ProgressServiceClient> _logger;
    public ProgressServiceClient(HttpClient client, ILogger<ProgressServiceClient> logger) { _client = client; _logger = logger; }
    public Task<ProgressSummary?> GetProgressSummaryAsync(Guid userId, CancellationToken cancellationToken = default) =>
        ClientHelpers.GetOrNullAsync<ProgressSummary>(_client, $"/api/v1/progress/{userId}/summary", _logger, cancellationToken);
}
