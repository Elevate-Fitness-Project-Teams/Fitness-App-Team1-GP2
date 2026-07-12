using MediatR;
using ProgressTrackingService.Common.Response;
using ProgressTrackingService.Features.Progress.WeightHistory.LogWeightHistory.Dto;
using ProgressTrackingService.Features.Progress.WeightHistory.LogWeightHistory.Request;

namespace ProgressTrackingService.Features.Progress.WeightHistory.LogWeightHistory.Command;

public record LogWeightCommand(LogWeightRequest Request, int UserId) : IRequest<ResponseResult<LogWeightDto>>;