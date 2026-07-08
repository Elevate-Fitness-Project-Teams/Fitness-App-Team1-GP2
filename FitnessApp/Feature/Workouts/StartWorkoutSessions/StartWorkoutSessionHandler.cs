using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkoutService.Common;
using WorkoutService.Contracts;
using WorkoutService.Contracts.Events;
using WorkoutService.Domain.Entities;
using WorkoutService.Domain.Enums;
using static System.Collections.Specialized.BitVector32;

namespace WorkoutService.Feature.Workouts.StartWorkoutSessions
{
    public class StartWorkoutSessionHandler(IUnitOfWork unitOfWork, IEventBus eventBus) : IRequestHandler<StartWorkoutSessionCommand, RequestResult<StartWorkoutSessionResponse>>
    {
        public async Task<RequestResult<StartWorkoutSessionResponse>> Handle(StartWorkoutSessionCommand request, CancellationToken cancellationToken)
        {
            var workoutExists = await unitOfWork
            .GetRepository<Workout>()
            .ExistsAsync(w => w.WorkoutId == request.workoutId, cancellationToken);

            if (!workoutExists)
            {
                return RequestResult<StartWorkoutSessionResponse>
                    .Failure(ErrorCode.WorkoutNotFound);
            }


            var exercises = await unitOfWork
             .GetRepository<WorkoutExercise>()
             .GetAll()
             .Where(we => we.WorkoutId == request.workoutId)
             .OrderBy(we => we.OrderIndex)
             .Select(we => new StartWorkoutExerciseResponse
             {
                 ExerciseId = we.ExerciseId,
                 Name = we.Exercise!.Name,
                 OrderIndex = we.OrderIndex,
                 RepsDefault = we.RepsDefault,
                 RestTimeInSeconds = we.RestTimeInSeconds,
                 SetsDefault = we.SetsDefault
             }).ToListAsync(cancellationToken);


                var existingSession = await unitOfWork
            .GetRepository<WorkoutSession>()
            .GetAll()
            .Where(x =>
                x.WorkoutId == request.workoutId &&
                x.UserId == request.userId &&
                x.Status == WorkoutSessionStatus.Active).FirstOrDefaultAsync(cancellationToken);


            if (existingSession != null)
            {
                var sessionResponse = new StartWorkoutSessionResponse
                {
                    SessionId = existingSession.SessionId,
                    IsNewSession = false,
                    WorkoutId = existingSession.WorkoutId,
                    UserId = existingSession.UserId,
                    Status = existingSession.Status.ToString(),
                    StartedAt = existingSession.StartedAt,
                    Exercises = exercises
                };
                return RequestResult<StartWorkoutSessionResponse>.Success(sessionResponse);
            }

            var newSession = new WorkoutSession
            {
                SessionId = Guid.NewGuid().ToString("N"),
                WorkoutId = request.workoutId,
                UserId = request.userId,
                Status = WorkoutSessionStatus.Active,
                StartedAt = DateTime.UtcNow,
            };

            await unitOfWork.GetRepository<WorkoutSession>().AddAsync(newSession, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);

            await eventBus.PublishAsync(new WorkoutSessionStartedEvent(
                newSession.SessionId,
                newSession.UserId,
                newSession.WorkoutId,
                newSession.StartedAt
            ), cancellationToken);


            var response = new StartWorkoutSessionResponse
            {
                SessionId = newSession.SessionId,
                IsNewSession = true,
                WorkoutId = newSession.WorkoutId,
                UserId = newSession.UserId,
                Status = newSession.Status.ToString(),
                StartedAt = newSession.StartedAt,
                Exercises = exercises
                
            };

            return RequestResult<StartWorkoutSessionResponse>.Success(response);
        }
    }
}
