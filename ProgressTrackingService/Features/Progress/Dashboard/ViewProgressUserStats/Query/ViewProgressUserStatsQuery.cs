using MediatR;
using ProgressTrackingService.Common;
using ProgressTrackingService.Common.Response;
using ProgressTrackingService.Features.Progress.Dashboard.ViewProgressUserStats.Dto;

namespace ProgressTrackingService.Features.Progress.Dashboard.ViewProgressUserStats.Query;

public record ViewProgressUserStatsQuery (int UserId): IRequest<ResponseResult<ViewProgressUserStatsDto>>;