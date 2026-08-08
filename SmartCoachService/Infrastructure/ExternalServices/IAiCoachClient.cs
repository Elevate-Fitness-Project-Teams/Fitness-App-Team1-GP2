namespace SmartCoachService.Infrastructure.ExternalServices;

public sealed class AiCoachReply
{
    public string Message { get; init; } = default!;
    public List<string> FollowUpSuggestions { get; init; } = new();
}

/// <summary>Wraps whatever LLM provider powers the coach (kept behind an interface so it's swappable / mockable in tests).</summary>
public interface IAiCoachClient
{
    Task<AiCoachReply> GetReplyAsync(string userMessage, string userContextPrompt, CancellationToken cancellationToken = default);
}
