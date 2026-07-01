using MediatR;
using WorkoutService.Common;

namespace WorkoutService.Feature.Workouts.GetWorkoutsByCategory
{
    public sealed record GetWorkoutsByCategoryQuery(string categoryName)
    : IRequest<RequestResult<List<GetWorkoutsByCategoryResponse>>>;
    
    

}
