using MediatR;
using Microsoft.Extensions.Caching.Memory;
using WorkoutService.Common;
using WorkoutService.Contracts;
using WorkoutService.Domain.Entities;

namespace WorkoutService.Feature.Workouts.GetWorkoutDetails
{
    public class GetWorkoutDetailsHandler(IUnitOfWork _unitOfWork,IMemoryCache memoryCache) : IRequestHandler<GetWorkoutDetailsQuery, RequestResult<GetWorkoutDetailsResponse>>
    {
        public async Task<RequestResult<GetWorkoutDetailsResponse>> Handle(GetWorkoutDetailsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"WorkoutDetails_{request.workoutId}";
            if(memoryCache.TryGetValue(cacheKey, out GetWorkoutDetailsResponse? cachedWorkout))
            {
                return RequestResult<GetWorkoutDetailsResponse>.Success(cachedWorkout!);
            }
            var workout = _unitOfWork.GetRepository<Workout>()
            .GetAll()
            .Where(w => w.WorkoutId == request.workoutId)
            .Select(x => new GetWorkoutDetailsResponse
            {
                WorkoutId = x.WorkoutId,
                Name = x.Name,
                Category = x.Category.ToString(),
                Difficulty = x.Difficulty.ToString(),
                DurationInMinutes = x.DurationInMinutes,
                PlanId = x.PlanId,
                PlanName = x.WorkoutPlan!.Name,

                Exercises = x.WorkoutExercises
                    .OrderBy(we => we.OrderIndex)
                    .Select(we => new WorkoutExerciseResponse
                    {
                        ExerciseId = we.ExerciseId,
                        Name = we.Exercise!.Name,
                        TargetMuscles = we.Exercise.TargetMuscles,
                        Equipment = we.Exercise.Equipment,
                        Difficulty = we.Exercise.Difficulty.ToString(),
                        Description = we.Exercise.Description,
                        VideoUrl = we.Exercise.VideoUrl,

                        SetsDefault = we.SetsDefault,
                        RepsDefault = we.RepsDefault,
                        RestTimeInSeconds = we.RestTimeInSeconds,
                        OrderIndex = we.OrderIndex
                    }).ToList()
            }).FirstOrDefault();


            if (workout is null)
            {
                return RequestResult<GetWorkoutDetailsResponse>.Failure(ErrorCode.WorkoutNotFound);
            }

            memoryCache.Set(cacheKey, workout, TimeSpan.FromMinutes(10));

            return RequestResult<GetWorkoutDetailsResponse>.Success(workout);

        }
    }
}

