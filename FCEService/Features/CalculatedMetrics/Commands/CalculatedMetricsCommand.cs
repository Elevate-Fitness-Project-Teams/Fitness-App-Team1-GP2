using FCEService.Common.Abstract;
using FCEService.Common.ViewModels;
using FCEService.Domain.Enums;
using FCEService.Domain.ValueObject;
using FCEService.Domain.Aggregates;
using MediatR;

namespace FCEService.Features.CalculatedMetrics.Commands
{
    public record CalculatedMetricsCommand(FCEService.Domain.Aggregates.CalculatedMetrics calculatedMetrics) : IRequest<RequestResult<bool>>;
    public class CalculatedMetricsCommandHandler : IRequestHandler<CalculatedMetricsCommand, RequestResult<bool>>
    {
        private readonly IGenericRepository<FCEService.Domain.Aggregates.CalculatedMetrics> _repository;
        public CalculatedMetricsCommandHandler(IGenericRepository<FCEService.Domain.Aggregates.CalculatedMetrics> repository) {
            _repository = repository;
        }
        public async Task<RequestResult<bool>> Handle(CalculatedMetricsCommand request, CancellationToken cancellationToken)
        {
            _repository.Add(request.calculatedMetrics);
            await _repository.SaveChangeAsync(cancellationToken);
            return RequestResult<bool>.Success(true, "Calculated metrics saved successfully.");
        }
    }

}
