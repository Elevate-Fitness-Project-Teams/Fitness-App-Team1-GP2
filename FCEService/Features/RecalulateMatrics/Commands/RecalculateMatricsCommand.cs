using FCEService.Common.Abstract;
using FCEService.Common.ViewModels;
using MediatR;

namespace FCEService.Features.RecalulateMatrics.Commands
{
    public record RecalculateMatricsCommand(FCEService.Domain.Aggregates.CalculatedMetrics metrics) : IRequest<RequestResult<bool>>;
    public class RecalculateMatricsCommandHandler : IRequestHandler<RecalculateMatricsCommand, RequestResult<bool>>
    {
        private readonly IGenericRepository<FCEService.Domain.Aggregates.CalculatedMetrics> _repository;
        public RecalculateMatricsCommandHandler(IGenericRepository<FCEService.Domain.Aggregates.CalculatedMetrics> repository)
        {
            _repository = repository;
        }
        public async Task<RequestResult<bool>> Handle(RecalculateMatricsCommand request, CancellationToken cancellationToken)
        {
            await _repository.SaveChangeAsync(cancellationToken);
            return RequestResult<bool>.Success(true, "Metrics recalculated successfully.");
        }
    }
}
