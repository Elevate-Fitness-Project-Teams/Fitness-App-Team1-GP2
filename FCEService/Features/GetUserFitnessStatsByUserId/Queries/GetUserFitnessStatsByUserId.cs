using FCEService.Common.Abstract;
using FCEService.Common.ViewModels;
using FCEService.Domain.Aggregates;
using FCEService.Domain.Enums;
using FCEService.Domain.ValueObject;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCEService.Features.GetUserFitnessStatsByUserId.Queries
{
    public record GetUserFitnessStatsByUserId(int UserId): IRequest<RequestResult<GetUserFitnessStatsResponseDTO>>;

    public class GetUserFitnessStatsByUserIdQueryHandler : IRequestHandler<GetUserFitnessStatsByUserId, RequestResult<GetUserFitnessStatsResponseDTO>>
    {
        private readonly IGenericRepository<FCEService.Domain.Aggregates.UserFitnessStats> _repository;

        public GetUserFitnessStatsByUserIdQueryHandler(IGenericRepository<UserFitnessStats> repository)
        {
            _repository = repository;
        }

        public async Task<RequestResult<GetUserFitnessStatsResponseDTO>> Handle(GetUserFitnessStatsByUserId request, CancellationToken cancellationToken)
        {
            var userFitnessStats = await _repository.Get(a=>a.userId==request.UserId)
                .Select(u=>new GetUserFitnessStatsResponseDTO(u.userId, u.physicalStats, u.goal, u.activity, u.WorkoutDays, u.IsActive))
                .AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            if(userFitnessStats == null)
            {
                return RequestResult<GetUserFitnessStatsResponseDTO>.Failure($"User fitness stats not found for userId: {request.UserId}", RequestErrorCode.NotFound);
            }
            return RequestResult<GetUserFitnessStatsResponseDTO>.Success(userFitnessStats, "User fitness stats retrieved successfully.");
        }
    }
    public record GetUserFitnessStatsResponseDTO(int userId, PhysicalStats physicalStats, Goal goal, Activity activity, int WorkoutDays, bool IsActive);
}
