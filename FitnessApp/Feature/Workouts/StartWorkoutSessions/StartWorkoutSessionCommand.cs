using MediatR;
using WorkoutService.Common;

namespace WorkoutService.Feature.Workouts.StartWorkoutSessions
{
    public sealed record StartWorkoutSessionCommand(
        int workoutId,
        int userId,
        string difficulty,
        int plannedDuration
    ) : IRequest<RequestResult<StartWorkoutSessionResponse>>;

}
