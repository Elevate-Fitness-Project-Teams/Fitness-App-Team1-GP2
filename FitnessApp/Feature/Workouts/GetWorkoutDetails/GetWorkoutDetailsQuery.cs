using MediatR;
using WorkoutService.Common;

namespace WorkoutService.Feature.Workouts.GetWorkoutDetails
{
    public sealed record GetWorkoutDetailsQuery(int workoutId) : IRequest<RequestResult<GetWorkoutDetailsResponse>>
    {
    }

}
