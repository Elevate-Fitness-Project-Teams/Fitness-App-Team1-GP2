using SmartCoachService.Application.Common.Interfaces;

namespace SmartCoachService.Infrastructure.ExternalServices;


public sealed class AiCoachService : IAiCoachService
{

    public Task<AiCoachReply> GetReplyAsync(
        string userMessage,
        UserContext context,
        IReadOnlyList<string> conversationHistory,
        CancellationToken ct = default)
    {

        var reply = new AiCoachReply(
            Message: $"[AI STUB] Received: '{userMessage}'. Real AI response goes here.",
            FollowUpSuggestions: new[]
            {
                "How many calories did I burn today?",
                "What should I eat before my workout?",
                "Show me my progress this week."
            });

        return Task.FromResult(reply);
    }
}
