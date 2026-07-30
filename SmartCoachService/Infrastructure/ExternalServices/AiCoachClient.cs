using System.Net.Http.Json;
using SmartCoachService.Common.Exceptions;

namespace SmartCoachService.Infrastructure.ExternalServices;

public sealed class AiCoachClient : IAiCoachClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AiCoachClient> _logger;

    public AiCoachClient(HttpClient httpClient, ILogger<AiCoachClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<AiCoachReply> GetReplyAsync(string userMessage, string userContextPrompt, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/v1/messages", new
            {
                system = userContextPrompt,
                message = userMessage
            }, cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new ServiceUnavailableException("The AI coaching provider is currently unavailable.");

            var reply = await response.Content.ReadFromJsonAsync<AiCoachReply>(cancellationToken: cancellationToken);
            return reply ?? new AiCoachReply { Message = "Sorry, I couldn't generate a reply just now.", FollowUpSuggestions = new() };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "AI coach provider call failed");
            throw new ServiceUnavailableException("The AI coaching provider is currently unavailable.");
        }
    }
}
