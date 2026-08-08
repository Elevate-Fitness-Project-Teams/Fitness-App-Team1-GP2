using FCEService.Common.Abstract;
using FCEService.Common.ViewModels;
using FCEService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCEService.Features.GetUsersPlanHistories.Queries
{
    public record GetUserPlanHistoryByPlanIdQuery() : IRequest<RequestResult<List<UserPlanHistoryDTO>>>;

    public class GetUsersPlanHistoriesQueryHandler : IRequestHandler<GetUserPlanHistoryByPlanIdQuery, RequestResult<List<UserPlanHistoryDTO>>>
    {
        private IGenericRepository<UserPlanHistory> _repository;
        public GetUsersPlanHistoriesQueryHandler(IGenericRepository<UserPlanHistory> repository)
        {
            _repository = repository;
        }
        public async Task<RequestResult<List<UserPlanHistoryDTO>>> Handle(GetUserPlanHistoryByPlanIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetAll().AsNoTracking().Select(x => new UserPlanHistoryDTO(
                x.UserId,
                x.ExternalPlanId,
                x.AssignedAt,
                x.EndedAt,
                x.ResonForChange
            )).ToListAsync(cancellationToken);
            if (result == null)
            {
                return RequestResult<List<UserPlanHistoryDTO>>.Failure("User plan history not found for the specified user.");
            }

            return RequestResult<List<UserPlanHistoryDTO>>.Success(result, "User plan history retrieved successfully.");
        }
    }
    public record UserPlanHistoryDTO(int UserId, int ExternalPlanId, DateTime AssignedAt, DateTime? EndedAt, string? ResonForChange);
}
