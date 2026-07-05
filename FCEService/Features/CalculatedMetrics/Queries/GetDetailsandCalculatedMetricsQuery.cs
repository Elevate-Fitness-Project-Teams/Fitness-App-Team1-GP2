using FCEService.Common.Abstract;
using FCEService.Common.ViewModels;
using FCEService.Domain.Enums;
using FCEService.Domain.ValueObject;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCEService.Features.CalculatedMetrics.Queries
{
    public record GetDetailsandCalculatedMetricsQuery(int userId) : IRequest<RequestResult<FCEService.Domain.Aggregates.CalculatedMetrics>>;
    public class GetDetailsandCalculatedMetricsQueryHandler : IRequestHandler<GetDetailsandCalculatedMetricsQuery, RequestResult<FCEService.Domain.Aggregates.CalculatedMetrics>>
    {
        private readonly IGenericRepository<FCEService.Domain.Aggregates.UserFitnessStats> _repository;
        public GetDetailsandCalculatedMetricsQueryHandler(IGenericRepository<FCEService.Domain.Aggregates.UserFitnessStats> repository)
        {
            _repository = repository;
        }
        public async Task<RequestResult<FCEService.Domain.Aggregates.CalculatedMetrics>> Handle(GetDetailsandCalculatedMetricsQuery request, CancellationToken cancellationToken)
        {
            var userFitnessStats = await _repository.Get(x => x.userId == request.userId && x.IsActive).FirstOrDefaultAsync();
            var calculatedMetrics = FCEService.Domain.Aggregates.CalculatedMetrics.Calculate(userFitnessStats);
            if (calculatedMetrics == null)
            {
                return RequestResult<FCEService.Domain.Aggregates.CalculatedMetrics>.Failure("Calculated metrics not found.");
            }
            return RequestResult<FCEService.Domain.Aggregates.CalculatedMetrics>.Success(calculatedMetrics, "Calculated metrics retrieved successfully.");
        }
    }

}
