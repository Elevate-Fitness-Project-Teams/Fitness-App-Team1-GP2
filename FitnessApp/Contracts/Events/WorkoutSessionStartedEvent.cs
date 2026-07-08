namespace WorkoutService.Contracts.Events
{
    public sealed record WorkoutSessionStartedEvent(
    string SessionId,
    int UserId,
    int WorkoutId,
    DateTime StartedAt
    );
    
}
