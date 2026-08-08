using FCEService.Common.Abstract;
using FCEService.Common.ViewModels;
using FCEService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCEService.Features.GetUserPlanHistoryByUserId.Queries
{
    public record GetUserPlanHistoryByIdQuery(int userId) : IRequest<RequestResult<List<UserPlanHistoryDTO>>>;

    public class GetUserPlanHistoryByIdQueryHandler : IRequestHandler<GetUserPlanHistoryByIdQuery, RequestResult<List<UserPlanHistoryDTO>>>
    {
        private IGenericRepository<UserPlanHistory> _repository;
        public GetUserPlanHistoryByIdQueryHandler(IGenericRepository<UserPlanHistory> repository)
        {
            _repository = repository;
        }
        public async Task<RequestResult<List<UserPlanHistoryDTO>>> Handle(GetUserPlanHistoryByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _repository.Get(x => x.UserId == request.userId).AsNoTracking().Select(x => new UserPlanHistoryDTO(
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
