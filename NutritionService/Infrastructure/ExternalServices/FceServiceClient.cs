using NutritionService.Common.Exceptions;
using NutritionService.Infrastructure.ExternalServices.Models;
using System.Net;
using System.Net.Http.Json;

namespace NutritionService.Infrastructure.ExternalServices;

public sealed class FceServiceClient : IFceServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FceServiceClient> _logger;

    public FceServiceClient(HttpClient httpClient, ILogger<FceServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<CalorieTargetResponse?> GetCalorieTargetAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/v1/fce/calorie-target/{userId}", cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!response.IsSuccessStatusCode)
                throw new ServiceUnavailableException("FCE Service returned a non-success status code.");

            return await response.Content.ReadFromJsonAsync<CalorieTargetResponse>(cancellationToken: cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to reach FCE Service for user {UserId}", userId);
            throw new ServiceUnavailableException("FCE Service is currently unavailable.");
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "FCE Service call timed out for user {UserId}", userId);
            throw new ServiceUnavailableException("FCE Service timed out.");
        }
    }
}
