using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkoutService.Common;
using WorkoutService.Contracts;
using WorkoutService.Domain.Entities;
using WorkoutService.Domain.Enums;

namespace WorkoutService.Feature.Workouts.GetWorkoutsByCategory
{
    public class GetWorkoutsByCategoryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetWorkoutsByCategoryQuery, RequestResult<List<GetWorkoutsByCategoryResponse>>>
    {
        public async Task<RequestResult<List<GetWorkoutsByCategoryResponse>>> Handle(GetWorkoutsByCategoryQuery request, CancellationToken cancellationToken)
        {
            var query = unitOfWork.GetRepository<Workout>();
            var category = ParseCategory(request.categoryName);

            if (category is null)
            {
                return RequestResult<List<GetWorkoutsByCategoryResponse>>
                    .Failure(ErrorCode.InvalidWorkoutCategory);
            }

            var workouts = await query
                .GetAll()
                .Where(x => x.Category == category.Value)
                .OrderBy(x => x.WorkoutId)
                .Select(x => new GetWorkoutsByCategoryResponse
                {
                    WorkoutId = x.WorkoutId,
                    Name = x.Name,
                    Category = x.Category.ToString(),
                    Difficulty = x.Difficulty.ToString(),
                    DurationInMinutes = x.DurationInMinutes,
                    PlanId = x.PlanId,
                    PlanName = x.WorkoutPlan!.Name
                })
                .ToListAsync(cancellationToken);
            if(workouts is null)
            {
                return RequestResult<List<GetWorkoutsByCategoryResponse>>
                    .Failure(ErrorCode.WorkoutNotFound);
            }

            return RequestResult<List<GetWorkoutsByCategoryResponse>>.Success(workouts);
        }
        #region Helper
        private static WorkoutCategory? ParseCategory(string categoryName)
        {
            var category = string.Concat(
                categoryName
                    .Trim()
                    .ToLowerInvariant()
                    .Where(char.IsLetterOrDigit)
            );
            return category switch
            {
                "fullbody" => WorkoutCategory.FullBody,
                "chest" => WorkoutCategory.Chest,
                "arms" => WorkoutCategory.Arms,
                "shoulders" => WorkoutCategory.Shoulders,
                "back" => WorkoutCategory.Back,
                "legs" => WorkoutCategory.Legs,
                "stomach" => WorkoutCategory.Stomach,
                "upperbody" => WorkoutCategory.UpperBody,
                "lowerbody" => WorkoutCategory.LowerBody,
                "push" => WorkoutCategory.Push,
                "pull" => WorkoutCategory.Pull,
                _ => null
            };
        }

        #endregion    
    }
}
