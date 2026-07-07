using FCEService.Common.Abstract;
using FCEService.Common.ViewModels;
using FCEService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCEService.Features.GetUserPlanHistoryByPlanId.Queries
{
    public record GetUserPlanHistoryByPlanIdQuery(int PlanId) : IRequest<RequestResult<List<UserPlanHistoryDTO>>>;

    public class GetUserPlanHistoryByPlanIdQueryHandler : IRequestHandler<GetUserPlanHistoryByPlanIdQuery, RequestResult<List<UserPlanHistoryDTO>>>
    {
        private IGenericRepository<UserPlanHistory> _repository;
        public GetUserPlanHistoryByPlanIdQueryHandler(IGenericRepository<UserPlanHistory> repository)
        {
            _repository = repository;
        }
        public async Task<RequestResult<List<UserPlanHistoryDTO>>> Handle(GetUserPlanHistoryByPlanIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _repository.Get(p => p.ExternalPlanId == request.PlanId).AsNoTracking().Select(x => new UserPlanHistoryDTO(
                x.UserId,
                x.ExternalPlanId,
                x.AssignedAt,
                x.EndedAt,
                x.ResonForChange
            )).ToListAsync(cancellationToken);
            if (result == null)
            {
                return RequestResult<List<UserPlanHistoryDTO>>.Failure("Plan history not found for the specified plan.");
            }

            return RequestResult<List<UserPlanHistoryDTO>>.Success(result, "Plan history retrieved successfully.");
        }
    }
    public record UserPlanHistoryDTO(int UserId, int ExternalPlanId, DateTime AssignedAt, DateTime? EndedAt, string? ResonForChange);
}
