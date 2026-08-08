using MassTransit;
using FitnessApp.Shared.Events;
using ProgressTrackingService.Domain.Entities;
using ProgressTrackingService.Infrastructure.Persistence.Context;

namespace ProgressTrackingService.Features.Progress.WorkoutLog.Consumer;

public class WorkoutSessionStartedConsumer(AppDbContext appDbContext) : IConsumer<WorkoutSessionStartedEvent>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task Consume(ConsumeContext<WorkoutSessionStartedEvent> context)
    {
        var message = context.Message;

        var session = new WorkoutSessionTracking
        {
            SessionId = message.SessionId,
            WorkoutId = message.WorkoutId,
            UserId = message.UserId,
            StartedAt = message.StartedAt
        };

        _appDbContext.WorkoutSessionTrackings.Add(session);

        await _appDbContext.SaveChangesAsync(context.CancellationToken);
    }
}