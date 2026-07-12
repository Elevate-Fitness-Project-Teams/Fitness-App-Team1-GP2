using MediatR;
using ProgressTrackingService.Common.Pagination;
using ProgressTrackingService.Common.Response;
using ProgressTrackingService.Features.Progress.WeightHistory.ViewWeightHistory.Dto;
using ProgressTrackingService.Features.Progress.WeightHistory.ViewWeightHistory.Query;
using ProgressTrackingService.Infrastructure.Persistence.Context;

namespace ProgressTrackingService.Features.Progress.WeightHistory.ViewWeightHistory.Handler;

public class ViewWeightQueryHandler(AppDbContext appDbContext) : IRequestHandler<ViewWeightQuery, ResponseResult<PaginatedResult<ViewWeightHistoryDto>>>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<ResponseResult<PaginatedResult<ViewWeightHistoryDto>>> Handle(ViewWeightQuery request, CancellationToken cancellationToken)
    {
        var weightHistory = _appDbContext.WeightHistories
            .Where(x => x.UserId == request.UserId)
            .OrderByDescending(x => x.Date)
            .Select(x => new ViewWeightHistoryDto
            {
                UserId = x.UserId,
                Weight = x.Weight,
                Date = x.Date,
                Notes = x.Notes
            });
        
        if (!weightHistory.Any())
            return ResponseResult<PaginatedResult<ViewWeightHistoryDto>>.Failure(StatusCode.NotFound, "No weight history records were found for this user.");
        
        var pagedResult = await weightHistory.ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
        
        return ResponseResult<PaginatedResult<ViewWeightHistoryDto>>.Success(pagedResult, "Weight history retrieved successfully.", StatusCode.Success);
    }
}