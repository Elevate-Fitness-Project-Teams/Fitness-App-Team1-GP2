namespace SmartCoachService.Application.Common.Interfaces;

public record UserContext(
    int DailyGoalCalories,
    decimal MinProteinGrams,
    string? FitnessGoal,
    decimal? CurrentWeightKg,
    decimal? BodyFatPercentage,
    int CompletedWorkoutsLast30Days);

public record AiCoachReply(
    string Message,
    IReadOnlyList<string> FollowUpSuggestions);

public interface IAiCoachService
{
    Task<AiCoachReply> GetReplyAsync(
        string userMessage,
        UserContext context,
        IReadOnlyList<string> conversationHistory,
        CancellationToken ct = default);
}
