using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProgressTrackingService.Common.Redis;
using ProgressTrackingService.Common.Response;
using ProgressTrackingService.Domain.Entities;
using ProgressTrackingService.Features.Progress.WeightHistory.LogWeightHistory.Command;
using ProgressTrackingService.Features.Progress.WeightHistory.LogWeightHistory.Dto;
using ProgressTrackingService.Features.Progress.WeightHistory.LogWeightHistory.Events;
using ProgressTrackingService.Infrastructure.Persistence.Context;

namespace ProgressTrackingService.Features.Progress.WeightHistory.LogWeightHistory.Handler;

public class LogWeightHandler (
    AppDbContext appDbContext,
    IPublishEndpoint publishEndpoint,
    RedisServices<LogWeightDto> redisServices)
    : IRequestHandler<LogWeightCommand, ResponseResult<LogWeightDto>>
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    private readonly RedisServices<LogWeightDto> _redisServices = redisServices;

    public async Task<ResponseResult<LogWeightDto>> Handle(LogWeightCommand request, CancellationToken cancellationToken)
    {
        // Last Weight Entry
        var lastWeightEntry = await _appDbContext.WeightHistories
            .Where(w => w.UserId == request.UserId)
            .OrderByDescending(w => w.Date)
            .FirstOrDefaultAsync(cancellationToken);
        
        decimal lastWeight = lastWeightEntry?.Weight ?? request.Request.Weight;
        
        // Insert New Weight Entry
        var weightHistory = new Domain.Entities.WeightHistory()
        {
            UserId = request.UserId,
            Weight = request.Request.Weight,
            Date = request.Request.Date,
            Notes = request.Request.Notes,
        };
        
        await _appDbContext.WeightHistories.AddAsync(weightHistory, cancellationToken);
        
        // Different Between Last Weight - Current Weight
        var differenceFromPrevious = lastWeight - request.Request.Weight;
        
        // Update User Statistics
        var userStatistic = await _appDbContext.UserStatistics
            .AsTracking()
            .FirstOrDefaultAsync(w => w.UserId == request.UserId, cancellationToken);

        if (userStatistic == null)
        {
            userStatistic = new UserStatistic()
            {
                UserId = request.UserId,
                TotalWeightLost = differenceFromPrevious > 0 ? differenceFromPrevious : 0,
                UpdatedAt = DateTimeOffset.UtcNow
            };
            
            await _appDbContext.UserStatistics.AddAsync(userStatistic, cancellationToken);
        }
        else
        {
            if (differenceFromPrevious > 0)
            {
                userStatistic.TotalWeightLost += differenceFromPrevious;
                userStatistic.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }

        var saved = await _appDbContext.SaveChangesAsync(cancellationToken);
        
        const decimal calculatedBmi = 22.5m;
        
        if (saved > 0)
        {
            // Publish LogWeight
            await _publishEndpoint.Publish(new LogWeightEvent()
            {
                UserId = request.UserId,
                Bmi = calculatedBmi,
                DifferenceFromPrevious = differenceFromPrevious,
                TotalWeightLost = userStatistic.TotalWeightLost,
            }, cancellationToken);
        }
        
        // Remove, If the user key placed from ViewUserProgress
        await _redisServices.RemoveRedisCacheAsync($"user_progress:{request.UserId}", cancellationToken);
        
        // Remove, If the user key placed from ViewProgressDashboard
        await _redisServices.RemoveRedisCacheAsync("progress_dashboard", cancellationToken);
        
        return ResponseResult<LogWeightDto>.Success(new LogWeightDto()
        {
            Bmi = calculatedBmi,
            DifferenceFromPrevious = differenceFromPrevious,
            TotalWeightLost = userStatistic.TotalWeightLost,
        }, $"{differenceFromPrevious} kg change registered.", StatusCode.Success);
    }
}