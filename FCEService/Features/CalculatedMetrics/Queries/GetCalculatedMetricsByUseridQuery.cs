using FCEService.Common.Abstract;
using FCEService.Common.ViewModels;
using FCEService.Domain.Enums;
using FCEService.Domain.ValueObject;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCEService.Features.CalculatedMetrics.Queries
{
    public record GetCalculatedMetricsByUseridQuery(int userId) : IRequest<RequestResult<calculatedMatricsDTO>>;
    public class GetCalculatedMetricsByUseridQueryQueryHandler : IRequestHandler<GetCalculatedMetricsByUseridQuery, RequestResult<calculatedMatricsDTO>>
    {
        private readonly IGenericRepository<FCEService.Domain.Aggregates.CalculatedMetrics> _repository;
        public GetCalculatedMetricsByUseridQueryQueryHandler(IGenericRepository<FCEService.Domain.Aggregates.CalculatedMetrics> repository)
        {
            _repository = repository;
        }
        public async Task<RequestResult<calculatedMatricsDTO>> Handle(GetCalculatedMetricsByUseridQuery request, CancellationToken cancellationToken)
        {
            var calculatedMetrics = await _repository.Get(x => x.UserId == request.userId).FirstOrDefaultAsync(cancellationToken);
            if (calculatedMetrics == null)
            {
                return RequestResult<calculatedMatricsDTO>.Failure("Calculated metrics not found for the specified user.");
            }
            return RequestResult<calculatedMatricsDTO>.Success(new calculatedMatricsDTO(
                calculatedMetrics.UserId,
                calculatedMetrics.BMR,
                calculatedMetrics.TDEE,
                calculatedMetrics.CalorieTarget,
                calculatedMetrics.BMRRange,
                calculatedMetrics.BMRStatus
            ), "Calculated metrics retrieved successfully.");
        }
    }
    public record calculatedMatricsDTO(int UserId, double BMR, double TDEE, double CalorieTarget, BMRRange BMRRange, BMRStatus BMRStatus);
}
