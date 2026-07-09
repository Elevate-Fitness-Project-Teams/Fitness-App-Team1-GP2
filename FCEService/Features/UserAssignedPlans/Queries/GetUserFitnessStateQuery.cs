using FCEService.Common.Abstract;
using FCEService.Common.ViewModels;
using FCEService.Domain.Aggregates;
using FCEService.Domain.Enums;
using FCEService.Domain.ValueObject;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCEService.Features.UserAssignedPlans.Queries
{
    public record GetUserFitnessStateQuery(int userId) : IRequest<RequestResult<UserFitnessStateDTO>>;
    public class GetUserFitnessStateQueryHandler : IRequestHandler<GetUserFitnessStateQuery, RequestResult<UserFitnessStateDTO>>
    {
        private readonly IGenericRepository<UserFitnessStats> _repository;
        public GetUserFitnessStateQueryHandler( IGenericRepository<UserFitnessStats> repository)
        {
            _repository = repository;
        }
        public async Task<RequestResult<UserFitnessStateDTO>> Handle(GetUserFitnessStateQuery request, CancellationToken cancellationToken)
        {
            var userFitnessStats = await _repository.Get(x => x.userId == request.userId&& x.IsActive)
                .Select(s => new UserFitnessStateDTO (s.userId, s.goal, s.IsActive)).FirstOrDefaultAsync(cancellationToken);
            return (userFitnessStats!=null)
                ? RequestResult<UserFitnessStateDTO>.Success(userFitnessStats, "User fitness state retrieved successfully.")
                : RequestResult<UserFitnessStateDTO>.Failure("User fitness state not found.",RequestErrorCode.NotFound);
        }
    }

    public record UserFitnessStateDTO(int userId, Goal goal,bool isActive);

}
