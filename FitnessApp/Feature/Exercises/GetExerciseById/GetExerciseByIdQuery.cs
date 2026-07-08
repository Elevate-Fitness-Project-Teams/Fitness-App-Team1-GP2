using MediatR;
using WorkoutService.Common;

namespace WorkoutService.Feature.Exercises.GetExerciseById;

public sealed record GetExerciseByIdQuery(int Id)
    : IRequest<RequestResult<GetExerciseByIdResponse>>;