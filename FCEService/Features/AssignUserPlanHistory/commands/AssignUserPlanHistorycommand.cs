using FCEService.Common.Abstract;
using FCEService.Common.ViewModels;
using MediatR;

namespace FCEService.Features.AssignUserPlanHistory.commands
{
    public record AssignUserPlanHistorycommand(int UserId, int ExternalPlanId, DateTime? AssignedAt, DateTime? EndedAt, string? ReasonForChange) : IRequest<RequestResult<bool>>;
    public class AssignUserPlanHistorycommandHandler : IRequestHandler<AssignUserPlanHistorycommand, RequestResult<bool>>
    {
        private readonly IGenericRepository<Domain.Entities.UserPlanHistory> _repository;
        public AssignUserPlanHistorycommandHandler(IGenericRepository<Domain.Entities.UserPlanHistory> repository)
        {
            _repository = repository;
        }
        public async Task<RequestResult<bool>> Handle(AssignUserPlanHistorycommand request, CancellationToken cancellationToken)
        {
            var assignedAt = request.AssignedAt ?? DateTime.UtcNow;
            var userPlanHistory = new Domain.Entities.UserPlanHistory
            {
                UserId = request.UserId,
                ExternalPlanId = request.ExternalPlanId,
                AssignedAt = assignedAt,
                EndedAt = request.EndedAt,
                ResonForChange = request.ReasonForChange
            };
            _repository.Add(userPlanHistory);
            await _repository.SaveChangeAsync(cancellationToken);
            return RequestResult<bool>.Success(true, "User plan history assigned successfully.");
        }
    }


}
