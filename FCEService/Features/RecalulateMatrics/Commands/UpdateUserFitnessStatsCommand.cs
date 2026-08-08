using FCEService.Common.Abstract;
using FCEService.Common.ViewModels;
using FCEService.Domain.Aggregates;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCEService.Features.RecalulateMatrics.Commands
{
    public record UpdateUserFitnessStatsCommand(int UserId, double NewWeight) : IRequest<RequestResult<bool>>;
    public class UpdateUserFitnessStatsCommandHandler : IRequestHandler<UpdateUserFitnessStatsCommand, RequestResult<bool>>
    {
        private readonly IGenericRepository<FCEService.Domain.Aggregates.UserFitnessStats> _repository;
        public UpdateUserFitnessStatsCommandHandler(IGenericRepository<FCEService.Domain.Aggregates.UserFitnessStats> repository)
        {
            _repository = repository;
        }
        public async Task<RequestResult<bool>> Handle(UpdateUserFitnessStatsCommand request, CancellationToken cancellationToken)
        {
            // Fetch the user's current fitness stats from the repository
            var userFitnessStats = await _repository.Get(u => u.userId == request.UserId).AsTracking().FirstOrDefaultAsync(cancellationToken);
            if (userFitnessStats == null)            
                return RequestResult<bool>.Failure("invalid Data.");
            userFitnessStats.UpdatedAt = DateTime.UtcNow;
            userFitnessStats.physicalStats = userFitnessStats.physicalStats with { Weight = request.NewWeight };

            //_repository.Update(userFitnessStats,
            //     nameof(UserFitnessStats.physicalStats.Weight),
            //      nameof(UserFitnessStats.UpdatedAt)                 
            //);
            await _repository.SaveChangeAsync(cancellationToken);
            return RequestResult<bool>.Success(true, "User fitness stats updated successfully.");
        }
    }
}
