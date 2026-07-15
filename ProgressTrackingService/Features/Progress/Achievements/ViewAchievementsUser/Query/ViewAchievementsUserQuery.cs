using MediatR;
using ProgressTrackingService.Common.Pagination;
using ProgressTrackingService.Common.Response;
using ProgressTrackingService.Features.Progress.Achievements.ViewAchievementsUser.Dto;

namespace ProgressTrackingService.Features.Progress.Achievements.ViewAchievementsUser.Query;

public record ViewAchievementsUserQuery (int UserId, int PageNumber = 1, int PageSize =10): IRequest<ResponseResult<PaginatedResult<UserAchievementDto>>>;