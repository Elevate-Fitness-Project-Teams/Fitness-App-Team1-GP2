using MediatR;
using ProgressTrackingService.Common.Response;
using ProgressTrackingService.Features.Progress.Dashboard.ViewProgressDashboard.Dto;

namespace ProgressTrackingService.Features.Progress.Dashboard.ViewProgressDashboard.Query;

public record ViewProgressDashboardQuery() : IRequest<ResponseResult<ViewProgressDashboardDto>>;