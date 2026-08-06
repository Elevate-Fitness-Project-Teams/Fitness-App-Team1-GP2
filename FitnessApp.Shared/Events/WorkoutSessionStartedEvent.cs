namespace FitnessApp.Shared.Events
{
    public sealed record WorkoutSessionStartedEvent(
     string SessionId,
     int UserId,
     int WorkoutId,
     DateTime StartedAt
 ) : IntegrationEvent;
}
