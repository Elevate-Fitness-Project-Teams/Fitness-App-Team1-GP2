using MediatR;
using ProgressTrackingService.Common.Response;
using ProgressTrackingService.Features.Progress.Dashboard.ViewUserProgress.Dto;

namespace ProgressTrackingService.Features.Progress.Dashboard.ViewUserProgress.Query;

public record ViewUserProgressQuery(int UserId) : IRequest<ResponseResult<ViewWeightHistoryDto>>;