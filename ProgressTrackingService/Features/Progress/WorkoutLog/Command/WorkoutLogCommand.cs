using MediatR;
using ProgressTrackingService.Common.Response;
using ProgressTrackingService.Features.Progress.WorkoutLog.Dto;
using ProgressTrackingService.Features.Progress.WorkoutLog.Request;

namespace ProgressTrackingService.Features.Progress.WorkoutLog.Command;

public record WorkoutLogCommand(int UserId, WorkoutLogRequest Request) : IRequest<ResponseResult<WorkoutLogDto>>;