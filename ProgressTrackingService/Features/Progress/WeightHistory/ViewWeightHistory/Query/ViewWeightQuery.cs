using MediatR;
using ProgressTrackingService.Common.Pagination;
using ProgressTrackingService.Common.Response;
using ProgressTrackingService.Features.Progress.WeightHistory.ViewWeightHistory.Dto;

namespace ProgressTrackingService.Features.Progress.WeightHistory.ViewWeightHistory.Query;

public record ViewWeightQuery(int UserId, int PageNumber = 1, int PageSize =10) : IRequest<ResponseResult<PaginatedResult<ViewWeightHistoryDto>>>;