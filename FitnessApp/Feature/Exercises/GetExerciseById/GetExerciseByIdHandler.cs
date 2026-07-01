using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkoutService.Common;
using WorkoutService.Contracts;
using WorkoutService.Domain.Entities;

namespace WorkoutService.Feature.Exercises.GetExerciseById;

public sealed class GetExerciseByIdHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetExerciseByIdQuery, RequestResult<GetExerciseByIdResponse>>
{
    public async Task<RequestResult<GetExerciseByIdResponse>> Handle(
        GetExerciseByIdQuery request,
        CancellationToken cancellationToken)
    {
        var exercise = await unitOfWork
            .GetRepository<Exercise>()
            .GetAll()
            .Where(x => x.ExerciseId == request.Id)
            .Select(x => new GetExerciseByIdResponse
            {
                ExerciseId = x.ExerciseId,
                Name = x.Name,
                TargetMuscles = x.TargetMuscles,
                Equipment = x.Equipment,
                Difficulty = x.Difficulty.ToString(),
                Description = x.Description,
                VideoUrl = x.VideoUrl
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (exercise is null)
        {
            return RequestResult<GetExerciseByIdResponse>
                .Failure(ErrorCode.ExerciseNotFound);
        }

        return RequestResult<GetExerciseByIdResponse>
            .Success(exercise);
    }
}